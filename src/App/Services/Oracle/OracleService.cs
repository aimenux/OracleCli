using System.Text;
using App.Configuration;
using App.Extensions;
using Dapper;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace App.Services.Oracle;

public class OracleService : IOracleService
{
    private readonly Settings _settings;

    public OracleService(IOptions<Settings> options)
    {
        _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<ICollection<OraclePackage>> GetOraclePackagesAsync(OracleParameters parameters, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT
                    AP.OWNER AS OwnerName, 
                    AP.OBJECT_NAME AS PackageName, 
                    AO.CREATED AS CreationDate, 
                    AO.LAST_DDL_TIME AS ModificationDate,
                    SUM(CASE WHEN AP.PROCEDURE_NAME IS NULL THEN 0 ELSE 1 END) AS ProceduresCount
                  FROM ALL_PROCEDURES AP
                  INNER JOIN ALL_OBJECTS AO ON AP.OBJECT_ID = AO.OBJECT_ID
                  WHERE 1 = 1
                    AND UPPER(AO.OBJECT_TYPE) = 'PACKAGE'
            """
        );

        if (!string.IsNullOrWhiteSpace(parameters.OwnerName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AP.OWNER) = :owner");
        }

        if (!string.IsNullOrWhiteSpace(parameters.FilterKeyword))
        {
            sqlBuilder.AppendLine($" AND ({HasKeyWord("AP.OWNER")} OR {HasKeyWord("AP.OBJECT_NAME")} OR {HasKeyWord("AP.PROCEDURE_NAME")})");
        }

        sqlBuilder.AppendLine(" GROUP BY AP.OWNER, AP.OBJECT_NAME, AO.CREATED, AO.LAST_DDL_TIME");
        sqlBuilder.AppendLine(" ORDER BY AP.OWNER, AP.OBJECT_NAME, AO.CREATED, AO.LAST_DDL_TIME ASC");

        var sql = $"WITH RWS AS ( {sqlBuilder} ) SELECT * FROM RWS WHERE ROWNUM <= :max";
        var sqlParameters = new
        {
            max = parameters.MaxItems + 1,
            owner = parameters.OwnerName?.ToUpper(),
            keyword = parameters.FilterKeyword?.ToUpper()
        };

        await using var connection = CreateOracleConnection(parameters);
        var oraclePackages = await connection.QueryAsync<OraclePackage>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
        return oraclePackages.ToList();
    }

    public async Task<ICollection<OracleFunction>> GetOracleFunctionsAsync(OracleParameters parameters, CancellationToken cancellationToken = default)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT
                    AP.OWNER AS OwnerName, 
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

        if (!string.IsNullOrWhiteSpace(parameters.OwnerName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AP.OWNER) = :owner");
        }

        if (!string.IsNullOrWhiteSpace(parameters.FilterKeyword))
        {
            sqlBuilder.AppendLine($" AND ({HasKeyWord("AP.OWNER")} OR {HasKeyWord("AP.OBJECT_NAME")})");
        }

        sqlBuilder.AppendLine(" ORDER BY AP.OWNER, AP.OBJECT_NAME ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = parameters.MaxItems + 1,
            owner = parameters.OwnerName?.ToUpper(),
            keyword = parameters.FilterKeyword?.ToUpper()
        };

        await using var connection = CreateOracleConnection(parameters);
        var oracleFunctions = await connection.QueryAsync<OracleFunction>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
        return oracleFunctions.ToList();
    }

    public async Task<ICollection<OracleProcedure>> GetOracleProceduresAsync(OracleParameters parameters, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT    
                    AP.OWNER AS OwnerName, 
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

        if (!string.IsNullOrWhiteSpace(parameters.OwnerName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AP.OWNER) = :owner");
        }

        if (!string.IsNullOrWhiteSpace(parameters.FilterKeyword))
        {
            sqlBuilder.AppendLine($" AND ({HasKeyWord("AP.OWNER")} OR {HasKeyWord("AP.OBJECT_NAME")} OR {HasKeyWord("AP.PROCEDURE_NAME")})");
        }

        sqlBuilder.AppendLine(" ORDER BY AP.OWNER, AP.OBJECT_NAME, AP.PROCEDURE_NAME ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = parameters.MaxItems + 1,
            owner = parameters.OwnerName?.ToUpper(),
            keyword = parameters.FilterKeyword?.ToUpper()
        };

        await using var connection = CreateOracleConnection(parameters);
        var oracleProcedures = await connection.QueryAsync<OracleProcedure>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
        return oracleProcedures.ToList();
    }

    public async Task<ICollection<OracleProcedure>> FindOracleProceduresAsync(OracleParameters parameters, CancellationToken cancellationToken = default)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT    
                    AP.OWNER AS OwnerName, 
                    (CASE WHEN AP.PROCEDURE_NAME IS NULL THEN '' ELSE AP.OBJECT_NAME END) AS PackageName, 
                    (CASE WHEN AP.PROCEDURE_NAME IS NULL THEN AP.OBJECT_NAME ELSE AP.PROCEDURE_NAME END) AS ProcedureName, 
                    AO.CREATED AS CreationDate,
                    AO.LAST_DDL_TIME AS ModificationDate
                  FROM ALL_PROCEDURES AP
                  INNER JOIN ALL_OBJECTS AO ON AP.OBJECT_ID = AO.OBJECT_ID
                  WHERE 1 = 1 
                    AND ((UPPER(AO.OBJECT_TYPE) = 'PROCEDURE' AND UPPER(AP.OBJECT_NAME) = :spcName) OR (UPPER(AO.OBJECT_TYPE) = 'PACKAGE' AND UPPER(AP.PROCEDURE_NAME) = :spcName))
                    AND ROWNUM <= :max
            """
        );

        if (!string.IsNullOrWhiteSpace(parameters.OwnerName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AP.OWNER) = :owner");
        }

        if (!string.IsNullOrWhiteSpace(parameters.PackageName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AP.OBJECT_NAME) = :pkgName");
        }

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = parameters.MaxItems + 1,
            owner = parameters.OwnerName?.ToUpper(),
            spcName = parameters.ProcedureName?.ToUpper(),
            pkgName = parameters.PackageName?.ToUpper()
        };

        await using var connection = CreateOracleConnection(parameters);
        var oracleProcedures = await connection.QueryAsync<OracleProcedure>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
        return oracleProcedures.ToList();
    }

    public async Task<ICollection<OracleArgument>> GetOracleArgumentsAsync(OracleParameters parameters, CancellationToken cancellationToken)
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

        if (!string.IsNullOrWhiteSpace(parameters.OwnerName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AA.OWNER) = :owner");
        }

        if (!string.IsNullOrWhiteSpace(parameters.ProcedureName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AA.OBJECT_NAME) = :procedure");
        }

        sqlBuilder.AppendLine(string.IsNullOrWhiteSpace(parameters.PackageName)
            ? " AND AA.PACKAGE_NAME IS NULL"
            : " AND UPPER(AA.PACKAGE_NAME) = :package");

        sqlBuilder.AppendLine(" ORDER BY AA.POSITION ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            owner = parameters.OwnerName?.ToUpper(),
            package = parameters.PackageName?.ToUpper(),
            procedure = parameters.ProcedureName?.ToUpper()
        };

        await using var connection = CreateOracleConnection(parameters);
        var oracleArguments = await connection.QueryAsync<OracleArgument>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
        return oracleArguments.ToList();
    }

    public async Task<ICollection<OracleSource>> GetOracleSourcesAsync(OracleParameters parameters, CancellationToken cancellationToken = default)
    {
        var hasOwnerName = !string.IsNullOrWhiteSpace(parameters.OwnerName)
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
                      WHERE {hasOwnerName}
                        AND UPPER(NAME) = :package
                        AND UPPER(TYPE) = 'PACKAGE BODY'
                        AND INSTR(UPPER(TEXT), :procedure) > 0
                        AND INSTR(UPPER(TEXT), 'PROCEDURE') > 0
                      ORDER BY LINE ASC
                  ),
                  LAST_LINE AS 
                  (
                      SELECT
                        LINE,
                        TEXT
                      FROM ALL_SOURCE
                      WHERE {hasOwnerName}
                        AND UPPER(NAME) = :package
                        AND UPPER(TYPE) = 'PACKAGE BODY'
                        AND INSTR(UPPER(TEXT), :procedure) > 0
                        AND INSTR(UPPER(TEXT), 'END') > 0
                      ORDER BY LINE ASC
                  )
                  SELECT AC.LINE AS Line, AC.TEXT AS Text
                    FROM ALL_SOURCE AC
                  WHERE {hasOwnerName}
                    AND UPPER(AC.NAME) = :package
                    AND UPPER(AC.TYPE) = 'PACKAGE BODY'
                    AND AC.LINE BETWEEN (SELECT LINE from FIRST_LINE) AND (SELECT LINE from LAST_LINE)
                  ORDER BY AC.LINE ASC
            """
        );

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            owner = parameters.OwnerName?.ToUpper(),
            package = parameters.PackageName?.ToUpper(),
            procedure = parameters.ProcedureName?.ToUpper()
        };

        await using var connection = CreateOracleConnection(parameters);
        var oracleSources = await connection.QueryAsync<OracleSource>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
        return oracleSources.ToList();
    }

    public async Task<ICollection<OracleObject>> GetOracleObjectsAsync(OracleParameters parameters, CancellationToken cancellationToken = default)
    {
        var getFromAllObjectsSourceTask = GetOracleObjectsFromAllObjectsSourceAsync(parameters, cancellationToken);
        var getPackagesFromAllProceduresSourceTask = GetOraclePackagesAsync(parameters, cancellationToken);
        var getProceduresFromAllProceduresSourceTask = GetOracleProceduresAsync(parameters, cancellationToken);
        var getFunctionsFromAllProceduresSourceTask = GetOracleFunctionsAsync(parameters, cancellationToken);

        await Task.WhenAll(
            getFromAllObjectsSourceTask,
            getPackagesFromAllProceduresSourceTask,
            getProceduresFromAllProceduresSourceTask,
            getFunctionsFromAllProceduresSourceTask);

        var objectsFromAllObjectsSource = (await getFromAllObjectsSourceTask)
            .Select(x => new OracleObject
            {
                OwnerName = x.OwnerName,
                ObjectName = x.ObjectName,
                ObjectType = x.ObjectType,
                CreationDate = x.CreationDate,
                ModificationDate = x.ModificationDate
            })
            .ToList();

        var packagesFromAllProceduresSource = (await getPackagesFromAllProceduresSourceTask)
            .Select(x => new OracleObject
            {
                OwnerName = x.OwnerName,
                ObjectName = x.PackageName,
                ObjectType = "PACKAGE",
                CreationDate = x.CreationDate,
                ModificationDate = x.ModificationDate
            })
            .ToList();

        var proceduresFromAllProceduresSource = (await getProceduresFromAllProceduresSourceTask)
            .Select(x => new OracleObject
            {
                OwnerName = x.OwnerName,
                ObjectName = string.IsNullOrWhiteSpace(x.ProcedureName) ? x.PackageName : x.ProcedureName,
                ObjectType = string.IsNullOrWhiteSpace(x.ProcedureName) ? "PACKAGE" : "PROCEDURE",
                CreationDate = x.CreationDate,
                ModificationDate = x.ModificationDate
            })
            .ToList();

        var functionsFromAllProceduresSource = (await getFunctionsFromAllProceduresSourceTask)
            .Select(x => new OracleObject
            {
                OwnerName = x.OwnerName,
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
            .OrderBy(x => x.OwnerName)
            .ThenBy(x => x.ObjectName)
            .ThenBy(x => x.ObjectType)
            .Take(parameters.MaxItems)
            .ToList();

        return objects;
    }

    public async Task<ICollection<OracleSchema>> GetOracleSchemasAsync(OracleParameters parameters, CancellationToken cancellationToken = default)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT 
                    AU.USERNAME AS SchemaName, AU.CREATED AS CreationDate 
                  FROM ALL_USERS AU
                  WHERE 1 = 1 
                    AND ROWNUM <= :max
            """
        );

        if (!string.IsNullOrWhiteSpace(parameters.FilterKeyword))
        {
            sqlBuilder.AppendLine($" AND {HasKeyWord("AU.USERNAME")}");
        }

        sqlBuilder.AppendLine(" ORDER BY AU.USERNAME ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = parameters.MaxItems + 1,
            keyword = parameters.FilterKeyword?.ToUpper()
        };

        await using var connection = CreateOracleConnection(parameters);
        var oracleSchemas = await connection.QueryAsync<OracleSchema>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
        return oracleSchemas.ToList();
    }

    private async Task<IEnumerable<OracleObject>> GetOracleObjectsFromAllObjectsSourceAsync(OracleParameters parameters, CancellationToken cancellationToken = default)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT 
                    AO.OWNER AS OwnerName, 
                    AO.OBJECT_NAME AS ObjectName, 
                    AO.OBJECT_TYPE AS ObjectType, 
                    AO.CREATED AS CreationDate,
                    AO.LAST_DDL_TIME AS ModificationDate
                  FROM ALL_OBJECTS AO
                  WHERE 1 = 1 
                    AND UPPER(AO.OBJECT_TYPE) IN ('FUNCTION', 'PROCEDURE', 'PACKAGE')
                    AND ROWNUM <= :max
            """
        );

        if (!string.IsNullOrWhiteSpace(parameters.OwnerName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AO.OWNER) = :owner");
        }

        if (!string.IsNullOrWhiteSpace(parameters.FilterKeyword))
        {
            sqlBuilder.AppendLine($" AND ({HasKeyWord("AO.OWNER")} OR {HasKeyWord("AO.OBJECT_NAME")} OR {HasKeyWord("AO.OBJECT_TYPE")})");
        }

        sqlBuilder.AppendLine(" ORDER BY AO.OWNER, AO.OBJECT_NAME, AO.OBJECT_TYPE ASC");

        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = parameters.MaxItems + 1,
            owner = parameters.OwnerName?.ToUpper(),
            keyword = parameters.FilterKeyword?.ToUpper()
        };

        await using var connection = CreateOracleConnection(parameters);
        var oracleObjects = await connection.QueryAsync<OracleObject>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
        return oracleObjects;
    }

    private OracleConnection CreateOracleConnection(OracleParameters parameters)
    {
        var connectionString = _settings.Databases
            .SingleOrDefault(x => x.DatabaseName.IgnoreEquals(parameters.DatabaseName))
            ?.ConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException($"ConnectionString not found for database name '{parameters.DatabaseName}'");
        }

        return new OracleConnection(connectionString);
    }

    private static string HasKeyWord(string fieldName) => $"UPPER({fieldName}) LIKE '%' || :keyword || '%'";
}