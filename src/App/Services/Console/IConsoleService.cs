using App.Services.Oracle;
using App.Validators;

namespace App.Services.Console;

public interface IConsoleService
{
    void CopyTextToClipboard(string text);
    void RenderTitle(string text);
    void RenderText(string text, string color = Colors.White);
    Task RenderStatusAsync(Func<Task> action);
    void RenderSettingsFile(string filepath);
    void RenderException(Exception exception);
    void RenderValidationErrors(ValidationErrors validationErrors);
    void RenderOracleObjects(ICollection<OracleObject> oracleObjects, OracleParameters parameters);
    void RenderOraclePackages(ICollection<OraclePackage> oraclePackages, OracleParameters parameters);
    void RenderOracleArguments(ICollection<OracleArgument> oracleArguments, OracleParameters parameters);
    void RenderOracleFunctions(ICollection<OracleFunction> oracleFunctions, OracleParameters parameters);
    void RenderOracleProcedures(ICollection<OracleProcedure> oracleProcedures, OracleParameters parameters);
    void CopyOracleArgumentsToClipboard(ICollection<OracleArgument> oracleArguments, OracleParameters parameters);
}