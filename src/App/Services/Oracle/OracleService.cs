using System.Text;
using App.Configuration;
using App.Extensions;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using Polly;

namespace App.Services.Oracle;

public class OracleService : IOracleService
{
    private readonly Settings _settings;
    private readonly ILogger<OracleService> _logger;

    public OracleService(IOptions<Settings> options, ILogger<OracleService> logger)
    {
        _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OracleInfo> GetOracleInfoAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var infoVersionTask = GetOracleInfoFromVersionAsync(oracleArgs, cancellationToken);
        var infoInstanceTask = GetOracleInfoFromInstanceAsync(oracleArgs, cancellationToken);
        var infoDatabaseTask = GetOracleInfoFromDatabaseAsync(oracleArgs, cancellationToken);
        await Task.WhenAll(infoVersionTask, infoInstanceTask, infoDatabaseTask);
        return new OracleInfo
        {
            Description = infoVersionTask.Result.Description,
            Version = infoInstanceTask.Result.Version,
            HostName = infoInstanceTask.Result.HostName,
            InstanceName = infoInstanceTask.Result.InstanceName,
            LogMode = infoDatabaseTask.Result.LogMode,
            OpenMode = infoDatabaseTask.Result.OpenMode,
            ProtectionMode = infoDatabaseTask.Result.ProtectionMode,
            InstanceStatus = infoInstanceTask.Result.InstanceStatus,
            DatabaseStatus = infoInstanceTask.Result.DatabaseStatus,
            CreationDate = infoDatabaseTask.Result.CreationDate,
            StartupDate = infoInstanceTask.Result.StartupDate
        };
    }

    public async Task<ICollection<OraclePackage>> GetOraclePackagesAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT
                    AO.OWNER AS SchemaName, 
                    AO.OBJECT_NAME AS PackageName, 
                    AO.CREATED AS CreationDate, 
                    AO.LAST_DDL_TIME AS ModificationDate
                  FROM ALL_OBJECTS AO
                  WHERE 1 = 1
                    AND UPPER(AO.OBJECT_TYPE) = 'PACKAGE'
                    AND ROWNUM <= :max
            """
        );

        if (!string.IsNullOrWhiteSpace(oracleArgs.SchemaName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AO.OWNER) = :owner");
        }
        
        if (!string.IsNullOrWhiteSpace(oracleArgs.PackageName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AO.OBJECT_NAME) = :package");
        }

        if (!string.IsNullOrWhiteSpace(oracleArgs.FilterKeyword))
        {
            sqlBuilder.AppendLine($" AND ({HasKeyWord("AO.OWNER")} OR {HasKeyWord("AO.OBJECT_NAME")})");
        }
        
        sqlBuilder.AppendLine(" ORDER BY AO.OWNER, AO.OBJECT_NAME, AO.CREATED, AO.LAST_DDL_TIME ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = oracleArgs.MaxItems + 1,
            owner = oracleArgs.SchemaName?.ToUpper(),
            package = oracleArgs.PackageName?.ToUpper(),
            keyword = oracleArgs.FilterKeyword?.ToUpper()
        };

        var retryPolicy = GetRetryPolicy<ICollection<OraclePackage>>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oraclePackages = await connection.QueryAsync<OraclePackage>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oraclePackages
                .Distinct()
                .OrderBy(x => x.SchemaName)
                .ThenBy(x => x.PackageName)
                .ToList();
        });
    }

    public async Task<ICollection<OracleFunction>> GetOracleFunctionsAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var getFromAllProceduresTask = GetOracleFunctionsFromAllProceduresAsync(oracleArgs, cancellationToken);
        var getFromAllArgumentsTask = GetOracleFunctionsFromAllArgumentsAsync(oracleArgs, cancellationToken);
        await Task.WhenAll(getFromAllProceduresTask, getFromAllArgumentsTask);
        var functionsFromAllProcedures = await getFromAllProceduresTask;
        var functionsFromAllSources = await getFromAllArgumentsTask;
        return functionsFromAllProcedures
            .Union(functionsFromAllSources)
            .Distinct()
            .OrderBy(x => x.SchemaName)
            .ThenBy(x => x.FunctionName)
            .Take(oracleArgs.MaxItems + 1)
            .ToList();
    }

    public async Task<ICollection<OracleProcedure>> GetOracleProceduresAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT    
                    AP.OWNER AS SchemaName, 
                    (CASE WHEN AP.PROCEDURE_NAME IS NULL THEN '' ELSE AP.OBJECT_NAME END) AS PackageName, 
                    (CASE WHEN AP.PROCEDURE_NAME IS NULL THEN AP.OBJECT_NAME ELSE AP.PROCEDURE_NAME END) AS ProcedureName, 
                    AO.CREATED AS CreationDate,
                    AO.LAST_DDL_TIME AS ModificationDate
                  FROM ALL_PROCEDURES AP
                  INNER JOIN ALL_OBJECTS AO ON AP.OBJECT_ID = AO.OBJECT_ID
                  WHERE 1 = 1 
                    AND (UPPER(AO.OBJECT_TYPE) = 'PROCEDURE' OR (UPPER(AO.OBJECT_TYPE) = 'PACKAGE' AND AP.PROCEDURE_NAME IS NOT NULL))
                    AND ROWNUM <= :max
            """
        );

        if (!string.IsNullOrWhiteSpace(oracleArgs.SchemaName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AP.OWNER) = :owner");
        }

        if (!string.IsNullOrWhiteSpace(oracleArgs.PackageName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AP.OBJECT_NAME) = :package");
        }
        
        if (!string.IsNullOrWhiteSpace(oracleArgs.ProcedureName))
        {
            sqlBuilder.AppendLine(" AND ((UPPER(AO.OBJECT_TYPE) = 'PROCEDURE' AND UPPER(AP.OBJECT_NAME) = :procedure) OR (UPPER(AO.OBJECT_TYPE) = 'PACKAGE' AND UPPER(AP.PROCEDURE_NAME) = :procedure))");
        }

        if (!string.IsNullOrWhiteSpace(oracleArgs.FilterKeyword))
        {
            sqlBuilder.AppendLine($" AND ({HasKeyWord("AP.OWNER")} OR {HasKeyWord("AP.OBJECT_NAME")} OR {HasKeyWord("AP.PROCEDURE_NAME")})");
        }

        sqlBuilder.AppendLine(" ORDER BY AP.OWNER, AP.OBJECT_NAME, AP.PROCEDURE_NAME ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = oracleArgs.MaxItems + 1,
            owner = oracleArgs.SchemaName?.ToUpper(),
            package = oracleArgs.PackageName?.ToUpper(),
            procedure = oracleArgs.ProcedureName?.ToUpper(),
            keyword = oracleArgs.FilterKeyword?.ToUpper()
        };

        var retryPolicy = GetRetryPolicy<ICollection<OracleProcedure>>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oracleProcedures = await connection.QueryAsync<OracleProcedure>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oracleProcedures
                .Distinct()
                .OrderBy(x => x.SchemaName)
                .ThenBy(x => x.ProcedureName)
                .ToList();
        });
    }

    public async Task<ICollection<OracleParameter>> GetOracleParametersAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT
                    AA.ARGUMENT_NAME AS Name, AA.POSITION AS Position, AA.DATA_TYPE AS DataType, AA.IN_OUT AS Direction
                  FROM ALL_ARGUMENTS AA
                  WHERE 1 = 1
            """
        );

        if (!string.IsNullOrWhiteSpace(oracleArgs.SchemaName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AA.OWNER) = :owner");
        }
        
        sqlBuilder.AppendLine(!string.IsNullOrWhiteSpace(oracleArgs.PackageName)
            ? " AND UPPER(AA.PACKAGE_NAME) = :package"
            : " AND AA.PACKAGE_NAME IS NULL");

        if (!string.IsNullOrWhiteSpace(oracleArgs.ProcedureName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AA.OBJECT_NAME) = :procedure");
        }
        
        if (!string.IsNullOrWhiteSpace(oracleArgs.FunctionName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AA.OBJECT_NAME) = :function");
        }

        sqlBuilder.AppendLine(" ORDER BY AA.POSITION ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            owner = oracleArgs.SchemaName?.ToUpper(),
            package = oracleArgs.PackageName?.ToUpper(),
            procedure = oracleArgs.ProcedureName?.ToUpper(),
            function = oracleArgs.FunctionName?.ToUpper()
        };

        var retryPolicy = GetRetryPolicy<ICollection<OracleParameter>>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oracleParameters = await connection.QueryAsync<OracleParameter>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oracleParameters
                .Distinct()
                .OrderBy(x => x.Position)
                .ToList();
        });
    }

    public async Task<ICollection<OracleSource>> GetOracleSourcesAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var unwrappedOracleSources = await GetUnwrappedOracleSourcesAsync(oracleArgs, cancellationToken);
        if (unwrappedOracleSources.Any())
        {
            return unwrappedOracleSources;
        }

        var wrappedOracleSources = await GetWrappedOracleSourcesAsync(oracleArgs, cancellationToken);
        return wrappedOracleSources;
    }

    public async Task<ICollection<OracleObject>> GetOracleObjectsAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var getFromAllObjectsSourceTask = GetOracleObjectsFromAllObjectsSourceAsync(oracleArgs, cancellationToken);
        
        var getPackagesFromAllProceduresSourceTask = oracleArgs.ObjectTypes.IgnoreContains("PACKAGE")
            ? GetOraclePackagesAsync(oracleArgs, cancellationToken)
            : GetCompletedTask(new List<OraclePackage>());
        
        var getProceduresFromAllProceduresSourceTask = oracleArgs.ObjectTypes.IgnoreContains("PROCEDURE") 
            ? GetOracleProceduresAsync(oracleArgs, cancellationToken)
            : GetCompletedTask(new List<OracleProcedure>());

        var getFunctionsFromAllProceduresSourceTask = oracleArgs.ObjectTypes.IgnoreContains("FUNCTION")
            ? GetOracleFunctionsAsync(oracleArgs, cancellationToken)
            : GetCompletedTask(new List<OracleFunction>());

        await Task.WhenAll(
            getFromAllObjectsSourceTask,
            getPackagesFromAllProceduresSourceTask,
            getProceduresFromAllProceduresSourceTask,
            getFunctionsFromAllProceduresSourceTask);

        var objectsFromAllObjectsSource = (await getFromAllObjectsSourceTask)
            .Select(x => new OracleObject
            {
                SchemaName = x.SchemaName,
                ObjectName = x.ObjectName,
                ObjectType = x.ObjectType,
                CreationDate = x.CreationDate,
                ModificationDate = x.ModificationDate
            })
            .ToList();

        var packagesFromAllProceduresSource = (await getPackagesFromAllProceduresSourceTask)
            .Select(x => new OracleObject
            {
                SchemaName = x.SchemaName,
                ObjectName = x.PackageName,
                ObjectType = "PACKAGE",
                CreationDate = x.CreationDate,
                ModificationDate = x.ModificationDate
            })
            .ToList();

        var proceduresFromAllProceduresSource = (await getProceduresFromAllProceduresSourceTask)
            .Select(x => new OracleObject
            {
                SchemaName = x.SchemaName,
                ObjectName = string.IsNullOrWhiteSpace(x.ProcedureName) ? x.PackageName : x.ProcedureName,
                ObjectType = string.IsNullOrWhiteSpace(x.ProcedureName) ? "PACKAGE" : "PROCEDURE",
                CreationDate = x.CreationDate,
                ModificationDate = x.ModificationDate
            })
            .ToList();

        var functionsFromAllProceduresSource = (await getFunctionsFromAllProceduresSourceTask)
            .Select(x => new OracleObject
            {
                SchemaName = x.SchemaName,
                ObjectName = x.FunctionName,
                ObjectType = "FUNCTION",
                CreationDate = x.CreationDate,
                ModificationDate = x.ModificationDate
            })
            .ToList();

        var objects = objectsFromAllObjectsSource
            .Union(packagesFromAllProceduresSource)
            .Union(proceduresFromAllProceduresSource)
            .Union(functionsFromAllProceduresSource)
            .Distinct()
            .OrderBy(x => x.SchemaName)
            .ThenBy(x => x.ObjectName)
            .Take(oracleArgs.MaxItems + 1)
            .ToList();

        return objects;
    }

    public async Task<ICollection<OracleSchema>> GetOracleSchemasAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT 
                    AU.USERNAME AS SchemaName, 
                    AU.CREATED AS CreationDate 
                  FROM ALL_USERS AU
                  WHERE 1 = 1 
                    AND ROWNUM <= :max
            """
        );
        
        if (!string.IsNullOrWhiteSpace(oracleArgs.SchemaName))
        {
            sqlBuilder.AppendLine($" AND UPPER(AU.USERNAME) = :owner");
        }

        if (!string.IsNullOrWhiteSpace(oracleArgs.FilterKeyword))
        {
            sqlBuilder.AppendLine($" AND {HasKeyWord("AU.USERNAME")}");
        }

        sqlBuilder.AppendLine(" ORDER BY AU.USERNAME ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = oracleArgs.MaxItems + 1,
            owner = oracleArgs.SchemaName?.ToUpper(),
            keyword = oracleArgs.FilterKeyword?.ToUpper()
        };
        
        var retryPolicy = GetRetryPolicy<ICollection<OracleSchema>>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oracleSchemas = await connection.QueryAsync<OracleSchema>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oracleSchemas
                .Distinct()
                .OrderBy(x => x.SchemaName)
                .ToList();
        });
    }

    public async Task<ICollection<OracleTable>> GetOracleTablesAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT 
                    AT.OWNER AS SchemaName, 
                    AT.TABLE_NAME AS TableName,
                    AT.NUM_ROWS AS RowsCount
                  FROM ALL_TABLES AT
                  WHERE 1 = 1 
                    AND ROWNUM <= :max
            """
        );
        
        if (!string.IsNullOrWhiteSpace(oracleArgs.SchemaName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AT.OWNER) = :owner");
        }

        if (!string.IsNullOrWhiteSpace(oracleArgs.FilterKeyword))
        {
            sqlBuilder.AppendLine($" AND ( {HasKeyWord("AT.OWNER")} OR {HasKeyWord("AT.TABLE_NAME")} )");
        }
        
        if (!string.IsNullOrWhiteSpace(oracleArgs.TableName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AT.TABLE_NAME) = :tabname");
        }

        sqlBuilder.AppendLine(" ORDER BY AT.OWNER, AT.TABLE_NAME ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = oracleArgs.MaxItems + 1,
            owner = oracleArgs.SchemaName?.ToUpper(),
            tabname = oracleArgs.TableName?.ToUpper(),
            keyword = oracleArgs.FilterKeyword?.ToUpper()
        };
        
        var retryPolicy = GetRetryPolicy<ICollection<OracleTable>>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oracleTables = await connection.QueryAsync<OracleTable>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oracleTables
                .Distinct()
                .OrderBy(x => x.SchemaName)
                .ThenBy(x => x.TableName)
                .ToList();
        });
    }

    public async Task<OracleTable> GetOracleTableAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT 
                    AC.COLUMN_NAME AS Name, 
                    AC.DATA_TYPE AS Type,
                    AC.COLUMN_ID AS Position,
                    AC.NULLABLE AS Nullable
                  FROM ALL_TAB_COLUMNS AC
                  INNER JOIN ALL_TABLES AT ON AC.TABLE_NAME = AT.TABLE_NAME AND AC.OWNER = AT.OWNER
                  WHERE 1 = 1 
                    AND UPPER(AT.TABLE_NAME) = :tabname
                    AND UPPER(AT.OWNER) = :owner
                    AND ROWNUM <= :max
                  ORDER BY AC.COLUMN_ID, AC.COLUMN_NAME ASC
            """
        );

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = oracleArgs.MaxItems + 1,
            owner = oracleArgs.SchemaName.ToUpper(),
            tabname = oracleArgs.TableName.ToUpper()
        };
        
        var retryPolicy = GetRetryPolicy<ICollection<OracleColumn>>();
        var tableColumns = await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oracleColumns = await connection.QueryAsync<OracleColumn>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oracleColumns
                .Distinct()
                .OrderBy(x => x.Position)
                .ToList();
        });

        return new OracleTable
        {
            TableName = oracleArgs.TableName,
            SchemaName = oracleArgs.SchemaName,
            TableColumns = tableColumns
        };
    }

    public async Task<ICollection<OracleLock>> GetOracleLocksAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT 
                    VS.SCHEMANAME AS SchemaName,
                    VS.OSUSER AS UserName,
                    VS.MACHINE AS MachineName,
                    VS.PROGRAM AS ProgramName,
                    VS.BLOCKING_SESSION AS BlockingSession,
                    VS.SID AS BlockedSession,
                    VS.SQL_EXEC_START AS BlockingStartDate,
                    VS.SECONDS_IN_WAIT AS BlockingTime,
                    VSQ.SQL_TEXT AS BlockedSqlText
                  FROM V$SESSION VS
                  INNER JOIN V$SQLAREA VSQ ON VS.SQL_ADDRESS = VSQ.ADDRESS
                  WHERE 1 = 1 
                    AND VS.BLOCKING_SESSION IS NOT NULL 
                    AND VS.SECONDS_IN_WAIT >= :time
                    AND ROWNUM <= :max
            """
        );
        
        if (!string.IsNullOrWhiteSpace(oracleArgs.SchemaName))
        {
            sqlBuilder.AppendLine(" AND UPPER(VS.SCHEMANAME) = :owner");
        }
        
        sqlBuilder.AppendLine(" ORDER BY VS.SECONDS_IN_WAIT DESC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = oracleArgs.MaxItems + 1,
            owner = oracleArgs.SchemaName?.ToUpper(),
            time = oracleArgs.MinBlockingTimeInMinutes * 60
        };
        
        var retryPolicy = GetRetryPolicy<ICollection<OracleLock>>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oracleLocks = await connection.QueryAsync<OracleLock>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oracleLocks
                .Distinct()
                .OrderByDescending(x => x.BlockingTime)
                .ToList();
        });
    }

    public async Task<ICollection<OracleSession>> GetOracleSessionsAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT 
                    VS.SCHEMANAME AS SchemaName,
                    VS.OSUSER AS UserName,
                    VS.MACHINE AS MachineName,
                    VS.PROGRAM AS ProgramName,
                    VS.MODULE AS ModuleName,
                    VS.State AS State,
                    VS.LOGON_TIME AS LogonDate,
                    VS.SQL_EXEC_START AS StartDate,
                    VSQ.SQL_TEXT AS SqlText
                  FROM V$SESSION VS
                  INNER JOIN V$SQLAREA VSQ ON VS.SQL_ADDRESS = VSQ.ADDRESS
                  WHERE 1 = 1
                    AND VS.STATUS = 'ACTIVE'
                    AND VS.USERNAME NOT LIKE 'SYS%'
                    AND ROWNUM <= :max
            """
        );
        
        if (!string.IsNullOrWhiteSpace(oracleArgs.SchemaName))
        {
            sqlBuilder.AppendLine(" AND UPPER(VS.SCHEMANAME) = :owner");
        }
        
        sqlBuilder.AppendLine(" ORDER BY VS.LOGON_TIME DESC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = oracleArgs.MaxItems + 1,
            owner = oracleArgs.SchemaName?.ToUpper()
        };
        
        var retryPolicy = GetRetryPolicy<ICollection<OracleSession>>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oracleSessions = await connection.QueryAsync<OracleSession>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oracleSessions
                .Distinct()
                .OrderByDescending(x => x.LogonDate)
                .ToList();
        });
    }
    
    private async Task<OracleInfo> GetOracleInfoFromVersionAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT 
                      BANNER AS Description
                  FROM V$VERSION
                  WHERE 1 = 1
                    AND UPPER(BANNER) LIKE '%ORACLE%'  
                    AND ROWNUM <= 1
            """
        );

        var sql = sqlBuilder.ToString();
        var retryPolicy = GetRetryPolicy<OracleInfo>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var infos = await connection.QueryAsync<OracleInfo>(sql, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return infos.SingleOrDefault();
        });
    }
    
    private async Task<OracleInfo> GetOracleInfoFromInstanceAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT 
                      HOST_NAME AS HostName,
                      INSTANCE_NAME AS InstanceName,
                      VERSION AS Version,
                      STATUS AS InstanceStatus,
                      DATABASE_STATUS AS DatabaseStatus,
                      STARTUP_TIME AS StartupDate
                  FROM V$INSTANCE
                  WHERE 1 = 1
                    AND ROWNUM <= 1
            """
        );

        var sql = sqlBuilder.ToString();
        var retryPolicy = GetRetryPolicy<OracleInfo>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var infos = await connection.QueryAsync<OracleInfo>(sql, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return infos.SingleOrDefault();
        });
    }
    
    private async Task<OracleInfo> GetOracleInfoFromDatabaseAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT 
                      LOG_MODE AS LogMode,
                      OPEN_MODE AS OpenMode,
                      PROTECTION_MODE AS ProtectionMode,
                      CREATED AS CreationDate
                  FROM V$DATABASE
                  WHERE 1 = 1
                    AND ROWNUM <= 1
            """
        );

        var sql = sqlBuilder.ToString();
        var retryPolicy = GetRetryPolicy<OracleInfo>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var infos = await connection.QueryAsync<OracleInfo>(sql, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return infos.SingleOrDefault();
        });
    }

    private async Task<ICollection<OracleSource>> GetUnwrappedOracleSourcesAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var name = !string.IsNullOrWhiteSpace(oracleArgs.ProcedureName)
            ? oracleArgs.ProcedureName
            : oracleArgs.FunctionName;
        
        var type = !string.IsNullOrWhiteSpace(oracleArgs.ProcedureName)
            ? "PROCEDURE"
            : "FUNCTION";
        
        var hasSchemaName = !string.IsNullOrWhiteSpace(oracleArgs.SchemaName)
            ? "UPPER(OWNER) = :owner"
            : "1 = 1";
        
        var sqlBuilder = new StringBuilder
        (
            $"""
                  WITH FIRST_LINE AS 
                  (
                      SELECT 
                        LINE,
                        TEXT
                      FROM ALL_SOURCE
                      WHERE {hasSchemaName}
                        AND UPPER(NAME) = :package
                        AND UPPER(TYPE) = 'PACKAGE BODY'
                        AND INSTR(UPPER(TEXT), :name) > 0
                        AND INSTR(UPPER(TEXT), '{type}') > 0
                        AND REGEXP_LIKE (UPPER(TEXT), :regex)
                      ORDER BY LINE ASC
                  ),
                  LAST_LINE AS 
                  (
                      SELECT
                        LINE,
                        TEXT
                      FROM ALL_SOURCE
                      WHERE {hasSchemaName}
                        AND UPPER(NAME) = :package
                        AND UPPER(TYPE) = 'PACKAGE BODY'
                        AND INSTR(UPPER(TEXT), :name) > 0
                        AND INSTR(UPPER(TEXT), 'END') > 0
                        AND REGEXP_LIKE (UPPER(TEXT), :regex)
                      ORDER BY LINE ASC
                  )
                  SELECT AC.LINE AS Line, AC.TEXT AS Text
                    FROM ALL_SOURCE AC
                  WHERE {hasSchemaName}
                    AND UPPER(AC.NAME) = :package
                    AND UPPER(AC.TYPE) = 'PACKAGE BODY'
                    AND AC.LINE BETWEEN (SELECT LINE from FIRST_LINE) AND (SELECT LINE from LAST_LINE)
                  ORDER BY AC.LINE ASC
            """
        );

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            name = name?.ToUpper(),
            owner = oracleArgs.SchemaName?.ToUpper(),
            package = oracleArgs.PackageName?.ToUpper(),
            regex = $@"{name?.ToUpper()}(;|\(|\s)+"
        };

        var retryPolicy = GetRetryPolicy<ICollection<OracleSource>>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oracleSources = await connection.QueryAsync<OracleSource>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oracleSources
                .Distinct()
                .OrderBy(x => x.Line)
                .ToList();
        });
    }
    
    private async Task<ICollection<OracleSource>> GetWrappedOracleSourcesAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var hasSchemaName = !string.IsNullOrWhiteSpace(oracleArgs.SchemaName)
            ? "UPPER(OWNER) = :owner"
            : "1 = 1";
        
        var sqlBuilder = new StringBuilder
        (
            $"""
                SELECT AC.LINE AS Line, AC.TEXT AS Text
                  FROM ALL_SOURCE AC
                WHERE {hasSchemaName}
                  AND UPPER(AC.NAME) = :package
                  AND UPPER(AC.TYPE) = 'PACKAGE BODY'
            """
        );

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            owner = oracleArgs.SchemaName?.ToUpper(),
            package = oracleArgs.PackageName?.ToUpper()
        };

        var retryPolicy = GetRetryPolicy<ICollection<OracleSource>>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oracleSources = await connection.QueryAsync<OracleSource>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oracleSources
                .Distinct()
                .OrderBy(x => x.Line)
                .ToList();
        });
    }
    
    private async Task<ICollection<OracleFunction>> GetOracleFunctionsFromAllProceduresAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT
                    AP.OWNER AS SchemaName, 
                    NULL AS PackageName,
                    AP.OBJECT_NAME AS FunctionName, 
                    AO.CREATED AS CreationDate,
                    AO.LAST_DDL_TIME AS ModificationDate
                  FROM ALL_PROCEDURES AP
                  INNER JOIN ALL_OBJECTS AO ON AP.OBJECT_ID = AO.OBJECT_ID
                  WHERE 1 = 1 
                    AND UPPER(AO.OBJECT_TYPE) = 'FUNCTION' 
                    AND ROWNUM <= :max
            """
        );

        if (!string.IsNullOrWhiteSpace(oracleArgs.SchemaName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AP.OWNER) = :owner");
        }
        
        if (!string.IsNullOrWhiteSpace(oracleArgs.PackageName))
        {
            sqlBuilder.AppendLine(" AND 1 = 2");
        }
        
        if (!string.IsNullOrWhiteSpace(oracleArgs.FunctionName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AP.OBJECT_NAME) = :function");
        }

        if (!string.IsNullOrWhiteSpace(oracleArgs.FilterKeyword))
        {
            sqlBuilder.AppendLine($" AND ({HasKeyWord("AP.OWNER")} OR {HasKeyWord("AP.OBJECT_NAME")})");
        }

        sqlBuilder.AppendLine(" ORDER BY AP.OWNER, AP.OBJECT_NAME ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = oracleArgs.MaxItems + 1,
            owner = oracleArgs.SchemaName?.ToUpper(),
            function = oracleArgs.FunctionName?.ToUpper(),
            keyword = oracleArgs.FilterKeyword?.ToUpper()
        };

        var retryPolicy = GetRetryPolicy<ICollection<OracleFunction>>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oracleFunctions = await connection.QueryAsync<OracleFunction>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oracleFunctions
                .Distinct()
                .OrderBy(x => x.SchemaName)
                .ThenBy(x => x.FunctionName)
                .ToList();
        });
    }
    
    private async Task<ICollection<OracleFunction>> GetOracleFunctionsFromAllArgumentsAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT DISTINCT 
                    AG.OWNER AS SchemaName, 
                    AG.PACKAGE_NAME AS PackageName, 
                    AG.OBJECT_NAME AS FunctionName, 
                    AO.CREATED AS CreationDate,
                    AO.LAST_DDL_TIME AS ModificationDate
                  FROM ALL_ARGUMENTS AG
                  INNER JOIN ALL_OBJECTS AO ON AG.OBJECT_ID = AO.OBJECT_ID
                  WHERE 1 = 1
                    AND AG.POSITION = 0
                    AND ROWNUM <= :max
            """
        );

        if (!string.IsNullOrWhiteSpace(oracleArgs.SchemaName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AG.OWNER) = :owner");
        }
        
        if (!string.IsNullOrWhiteSpace(oracleArgs.PackageName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AG.PACKAGE_NAME) = :package");
        }
        
        if (!string.IsNullOrWhiteSpace(oracleArgs.FunctionName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AG.OBJECT_NAME) = :function");
        }

        if (!string.IsNullOrWhiteSpace(oracleArgs.FilterKeyword))
        {
            sqlBuilder.AppendLine($" AND ({HasKeyWord("AG.OWNER")} OR {HasKeyWord("AG.PACKAGE_NAME")} OR {HasKeyWord("AG.OBJECT_NAME")})");
        }

        sqlBuilder.AppendLine(" ORDER BY AG.OWNER, AG.PACKAGE_NAME, AG.OBJECT_NAME ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = oracleArgs.MaxItems + 1,
            owner = oracleArgs.SchemaName?.ToUpper(),
            package = oracleArgs.PackageName?.ToUpper(),
            function = oracleArgs.FunctionName?.ToUpper(),
            keyword = oracleArgs.FilterKeyword?.ToUpper()
        };

        var retryPolicy = GetRetryPolicy<ICollection<OracleFunction>>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oracleFunctions = await connection.QueryAsync<OracleFunction>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oracleFunctions
                .Distinct()
                .OrderBy(x => x.SchemaName)
                .ThenBy(x => x.FunctionName)
                .ToList();
        });
    }

    private async Task<ICollection<OracleObject>> GetOracleObjectsFromAllObjectsSourceAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT 
                    AO.OWNER AS SchemaName, 
                    AO.OBJECT_NAME AS ObjectName, 
                    AO.OBJECT_TYPE AS ObjectType, 
                    AO.CREATED AS CreationDate,
                    AO.LAST_DDL_TIME AS ModificationDate
                  FROM ALL_OBJECTS AO
                  WHERE 1 = 1 
                    AND ROWNUM <= :max
            """
        );

        if (!string.IsNullOrWhiteSpace(oracleArgs.SchemaName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AO.OWNER) = :owner");
        }

        if (!string.IsNullOrWhiteSpace(oracleArgs.FilterKeyword))
        {
            sqlBuilder.AppendLine($" AND ({HasKeyWord("AO.OWNER")} OR {HasKeyWord("AO.OBJECT_NAME")} OR {HasKeyWord("AO.OBJECT_TYPE")})");
        }

        if (oracleArgs.ObjectTypes?.Any() == true)
        {
            sqlBuilder.AppendLine(" AND UPPER(AO.OBJECT_TYPE) IN :types");
        }

        sqlBuilder.AppendLine(" ORDER BY AO.OWNER, AO.OBJECT_NAME, AO.OBJECT_TYPE ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = oracleArgs.MaxItems + 1,
            owner = oracleArgs.SchemaName?.ToUpper(),
            keyword = oracleArgs.FilterKeyword?.ToUpper(),
            types = oracleArgs.ObjectTypes?.Select(x => x.ToUpper())
        };

        var retryPolicy = GetRetryPolicy<ICollection<OracleObject>>();
        return await retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = CreateOracleConnection(oracleArgs);
            var oracleObjects = await connection.QueryAsync<OracleObject>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
            return oracleObjects
                .Distinct()
                .OrderBy(x => x.SchemaName)
                .ThenBy(x => x.ObjectName)
                .ToList();
        });
    }

    private OracleConnection CreateOracleConnection(OracleArgs oracleArgs)
    {
        if (oracleArgs.DatabaseName.IsConnectionString())
        {
            return new OracleConnection(oracleArgs.DatabaseName);
        }
        
        var connectionString = _settings.Databases
            .SingleOrDefault(x => x.DatabaseName.IgnoreEquals(oracleArgs.DatabaseName))
            ?.ConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException($"ConnectionString not found for database name '{oracleArgs.DatabaseName}'");
        }

        return new OracleConnection(connectionString);
    }
    
    private IAsyncPolicy<T> GetRetryPolicy<T>()
    {
        var maxRetry = _settings.MaxRetry;
        var sleepDuration = TimeSpan.FromSeconds(1);
        var retryPolicy = Policy<T>
            .Handle<OracleException>()
            .WaitAndRetryAsync(maxRetry,
                _ => sleepDuration,
                (response, _, retryCount, _) =>
                {
                    OnRetry(response, retryCount, maxRetry);
                });
        return retryPolicy;
    }
    
    private void OnRetry<T>(DelegateResult<T> response, int retryCount, int maxRetry)
    {
        var reason = response.Exception?.Message;
        if (string.IsNullOrWhiteSpace(reason))
        {
            _logger.LogTrace("Retry attempt {RetryCount}/{MaxRetry}", 
                retryCount, 
                maxRetry);
        }
        else
        {
            _logger.LogTrace("Retry attempt {RetryCount}/{MaxRetry}: {Reason}",
                retryCount,
                maxRetry,
                reason);
        }
    }
    
    private static Task<ICollection<T>> GetCompletedTask<T>(ICollection<T> result)
    {
        var taskSource = new TaskCompletionSource<ICollection<T>>();
        taskSource.SetResult(result);
        return taskSource.Task;
    }

    private static string HasKeyWord(string fieldName) => $"UPPER({fieldName}) LIKE '%' || :keyword || '%'";
}