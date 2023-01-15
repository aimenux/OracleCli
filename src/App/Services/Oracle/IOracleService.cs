namespace App.Services.Oracle;

public interface IOracleService
{
    Task<ICollection<OraclePackage>> GetOraclePackagesAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<ICollection<OracleFunction>> GetOracleFunctionsAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<ICollection<OracleProcedure>> GetOracleProceduresAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<ICollection<OracleProcedure>> FindOracleProceduresAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<ICollection<OracleArgument>> GetOracleArgumentsAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<ICollection<OracleSource>> GetOracleSourcesAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<ICollection<OracleObject>> GetOracleObjectsAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<ICollection<OracleSchema>> GetOracleSchemasAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
}