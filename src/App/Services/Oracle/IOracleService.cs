namespace App.Services.Oracle;

public interface IOracleService
{
    Task<OracleInfo> GetOracleInfoAsync(OracleArgs oracleArgs, CancellationToken cancellationToken);
    Task<ICollection<OraclePackage>> GetOraclePackagesAsync(OracleArgs oracleArgs, CancellationToken cancellationToken);
    Task<ICollection<OracleFunction>> GetOracleFunctionsAsync(OracleArgs oracleArgs, CancellationToken cancellationToken);
    Task<ICollection<OracleProcedure>> GetOracleProceduresAsync(OracleArgs oracleArgs, CancellationToken cancellationToken);
    Task<ICollection<OracleParameter>> GetOracleParametersAsync(OracleArgs oracleArgs, CancellationToken cancellationToken);
    Task<ICollection<OracleSource>> GetOracleSourcesAsync(OracleArgs oracleArgs, CancellationToken cancellationToken);
    Task<ICollection<OracleObject>> GetOracleObjectsAsync(OracleArgs oracleArgs, CancellationToken cancellationToken);
    Task<ICollection<OracleSchema>> GetOracleSchemasAsync(OracleArgs oracleArgs, CancellationToken cancellationToken);
    Task<ICollection<OracleTable>> GetOracleTablesAsync(OracleArgs oracleArgs, CancellationToken cancellationToken);
    Task<OracleTable> GetOracleTableAsync(OracleArgs oracleArgs, CancellationToken cancellationToken);
    Task<ICollection<OracleLock>> GetOracleLocksAsync(OracleArgs oracleArgs, CancellationToken cancellationToken);
    Task<ICollection<OracleSession>> GetOracleSessionsAsync(OracleArgs oracleArgs, CancellationToken cancellationToken);
}