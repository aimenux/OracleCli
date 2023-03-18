namespace App.Services.Oracle;

public interface IOracleService
{
    Task<OracleInfo> GetOracleInfoAsync(OracleArgs args, CancellationToken cancellationToken);
    Task<ICollection<OraclePackage>> GetOraclePackagesAsync(OracleArgs args, CancellationToken cancellationToken);
    Task<ICollection<OracleFunction>> GetOracleFunctionsAsync(OracleArgs args, CancellationToken cancellationToken);
    Task<ICollection<OracleProcedure>> GetOracleProceduresAsync(OracleArgs args, CancellationToken cancellationToken);
    Task<ICollection<OracleArgument>> GetOracleArgumentsAsync(OracleArgs args, CancellationToken cancellationToken);
    Task<ICollection<OracleSource>> GetOracleSourcesAsync(OracleArgs args, CancellationToken cancellationToken);
    Task<ICollection<OracleObject>> GetOracleObjectsAsync(OracleArgs args, CancellationToken cancellationToken);
    Task<ICollection<OracleSchema>> GetOracleSchemasAsync(OracleArgs args, CancellationToken cancellationToken);
    Task<ICollection<OracleTable>> GetOracleTablesAsync(OracleArgs args, CancellationToken cancellationToken);
    Task<OracleTable> GetOracleTableAsync(OracleArgs args, CancellationToken cancellationToken);
    Task<ICollection<OracleLock>> GetOracleLocksAsync(OracleArgs args, CancellationToken cancellationToken);
    Task<ICollection<OracleSession>> GetOracleSessionsAsync(OracleArgs args, CancellationToken cancellationToken);
}