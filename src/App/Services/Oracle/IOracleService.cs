namespace App.Services.Oracle;

public interface IOracleService
{
    Task<IEnumerable<OraclePackage>> GetOraclePackagesAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<IEnumerable<OracleFunction>> GetOracleFunctionsAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<IEnumerable<OracleProcedure>> GetOracleProceduresAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<IEnumerable<OracleProcedure>> FindOracleProceduresAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<IEnumerable<OracleArgument>> GetOracleArgumentsAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<IEnumerable<OracleObject>> GetOracleObjectsAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
}