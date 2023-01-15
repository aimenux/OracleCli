using App.Services.Oracle;
using App.Validators;

namespace App.Services.Console;

public interface IConsoleService
{
    void RenderTitle(string text);
    void RenderVersion(string version);
    void RenderText(string text, string color = Colors.White);
    void RenderSettingsFile(string filepath);
    void RenderException(Exception exception);
    Task RenderStatusAsync(Func<Task> action);
    Task<T> RenderStatusAsync<T>(Func<Task<T>> func);
    void RenderValidationErrors(ValidationErrors validationErrors);
    Task CopyTextToClipboardAsync(string text, CancellationToken cancellationToken);
    void RenderOracleObjects(ICollection<OracleObject> oracleObjects, OracleParameters parameters);
    void RenderOracleSchemas(ICollection<OracleSchema> oracleSchemas, OracleParameters parameters);
    void RenderOracleSources(ICollection<OracleSource> oracleSources, OracleParameters parameters);
    void RenderOraclePackages(ICollection<OraclePackage> oraclePackages, OracleParameters parameters);
    void RenderOracleArguments(ICollection<OracleArgument> oracleArguments, OracleParameters parameters);
    void RenderOracleFunctions(ICollection<OracleFunction> oracleFunctions, OracleParameters parameters);
    void RenderOracleProcedures(ICollection<OracleProcedure> oracleProcedures, OracleParameters parameters);
}