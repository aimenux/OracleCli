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
    
    public async Task<IEnumerable<OraclePackage>> GetOraclePackagesAsync(OracleParameters parameters, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT
                    AP.OWNER AS OwnerName, AP.OBJECT_NAME AS PackageName, COUNT(*) AS ProceduresCount
                  FROM ALL_PROCEDURES AP
                  INNER JOIN ALL_OBJECTS AO ON AP.OBJECT_ID = AO.OBJECT_ID
                  WHERE 1 = 1
                    AND UPPER(AO.OBJECT_TYPE) = 'PACKAGE' 
                    AND AP.PROCEDURE_NAME IS NOT NULL
            """
        );

        if (!string.IsNullOrWhiteSpace(parameters.OwnerName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AP.OWNER) = :owner");
        }
        
        if (!string.IsNullOrWhiteSpace(parameters.Keyword))
        {
            sqlBuilder.AppendLine($" AND ({HasKeyWord("AP.OWNER")} OR {HasKeyWord("AP.OBJECT_NAME")} OR {HasKeyWord("AP.PROCEDURE_NAME")})");
        }
        
        sqlBuilder.AppendLine(" GROUP BY AP.OWNER, AP.OBJECT_NAME");
        sqlBuilder.AppendLine(" ORDER BY AP.OWNER, AP.OBJECT_NAME ASC");
        
        var sql = $"WITH RWS AS ( {sqlBuilder} ) SELECT * FROM RWS WHERE ROWNUM <= :max";
        var sqlParameters = new
        {
            max = parameters.MaxItems + 1,
            owner = parameters.OwnerName?.ToUpper(),
            keyword = parameters.Keyword?.ToUpper()
        };
        
        await using var connection = CreateOracleConnection(parameters);
        var oraclePackages = await connection.QueryAsync<OraclePackage>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
        return oraclePackages;
    }

    public async Task<IEnumerable<OracleArgument>> GetOracleArgumentsAsync(OracleParameters parameters, CancellationToken cancellationToken)
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
        
        if (!string.IsNullOrWhiteSpace(parameters.PackageName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AA.PACKAGE_NAME) = :package");
        }
        
        if (!string.IsNullOrWhiteSpace(parameters.ProcedureName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AA.OBJECT_NAME) = :procedure");
        }
        
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
        return oracleArguments;
    }

    public async Task<IEnumerable<OracleProcedure>> GetOracleProceduresAsync(OracleParameters parameters, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder
        (
            """
                  SELECT
                    AP.OWNER AS OwnerName, AP.OBJECT_NAME AS PackageName, AP.PROCEDURE_NAME AS ProcedureName, AO.CREATED AS CreationDate
                  FROM ALL_PROCEDURES AP
                  INNER JOIN ALL_OBJECTS AO ON AP.OBJECT_ID = AO.OBJECT_ID
                  WHERE 1 = 1 
                    AND UPPER(AO.OBJECT_TYPE) = 'PACKAGE' 
                    AND AP.PROCEDURE_NAME IS NOT NULL
                    AND ROWNUM <= :max
            """
        );

        if (!string.IsNullOrWhiteSpace(parameters.OwnerName))
        {
            sqlBuilder.AppendLine(" AND UPPER(AP.OWNER) = :owner");
        }
        
        if (!string.IsNullOrWhiteSpace(parameters.Keyword))
        {
            sqlBuilder.AppendLine($" AND ({HasKeyWord("AP.OWNER")} OR {HasKeyWord("AP.OBJECT_NAME")} OR {HasKeyWord("AP.PROCEDURE_NAME")})");
        }

        sqlBuilder.AppendLine(" ORDER BY AP.OWNER, AP.OBJECT_NAME, AP.PROCEDURE_NAME ASC");
        
        var sql = sqlBuilder.ToString();
        var sqlParameters = new
        {
            max = parameters.MaxItems + 1,
            owner = parameters.OwnerName?.ToUpper(),
            keyword = parameters.Keyword?.ToUpper()
        };
        
        await using var connection = CreateOracleConnection(parameters);
        var oracleStoredProcedures = await connection.QueryAsync<OracleProcedure>(sql, sqlParameters, commandTimeout: Settings.DatabaseTimeoutInSeconds);
        return oracleStoredProcedures;
    }

    private OracleConnection CreateOracleConnection(OracleParameters parameters)
    {
        var connectionString = _settings.Databases
            .SingleOrDefault(x => x.DatabaseName.IgnoreEquals(parameters.DatabaseName))
            ?.ConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException($"ConnectionString not found for database name {parameters.DatabaseName}");
        }

        return new OracleConnection(connectionString);
    }
    
    private static string HasKeyWord(string fieldName) => $"UPPER({fieldName}) LIKE '%' || :keyword || '%'";
}