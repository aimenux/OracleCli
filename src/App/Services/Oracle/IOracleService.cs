namespace App.Services.Oracle;

public interface IOracleService
{
    Task<IEnumerable<OraclePackage>> GetOraclePackagesAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<IEnumerable<OracleArgument>> GetOracleArgumentsAsync(OracleParameters parameters, CancellationToken cancellationToken = default);
    Task<IEnumerable<OracleProcedure>> GetOracleProceduresAsync(OracleParameters parameters, CancellationToken cancellationToken = default);

}