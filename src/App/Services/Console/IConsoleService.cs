using App.Services.Oracle;
using App.Validators;

namespace App.Services.Console;

public interface IConsoleService
{
    void RenderTitle(string text);
    Task RenderStatusAsync(Func<Task> action);
    void RenderSettingsFile(string filepath);
    void RenderException(Exception exception);
    void RenderValidationErrors(ValidationErrors validationErrors);
    void RenderOraclePackages(IEnumerable<OraclePackage> oraclePackages, OracleParameters parameters);
    void RenderOracleArguments(IEnumerable<OracleArgument> oracleArguments, OracleParameters parameters);
    void RenderOracleProcedures(IEnumerable<OracleProcedure> oracleProcedures, OracleParameters parameters);
}