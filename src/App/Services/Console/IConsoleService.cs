using App.Services.Oracle;
using App.Validators;

namespace App.Services.Console;

public interface IConsoleService
{
    void RenderTitle(string text);
    void RenderText(string text, string color = Colors.White);
    Task RenderStatusAsync(Func<Task> action);
    void RenderSettingsFile(string filepath);
    void RenderException(Exception exception);
    void RenderValidationErrors(ValidationErrors validationErrors);
    void RenderOracleObjects(IEnumerable<OracleObject> oracleObjects, OracleParameters parameters);
    void RenderOraclePackages(IEnumerable<OraclePackage> oraclePackages, OracleParameters parameters);
    void RenderOracleArguments(IEnumerable<OracleArgument> oracleArguments, OracleParameters parameters);
    void RenderOracleFunctions(IEnumerable<OracleFunction> oracleFunctions, OracleParameters parameters);
    void RenderOracleProcedures(IEnumerable<OracleProcedure> oracleProcedures, OracleParameters parameters);
}