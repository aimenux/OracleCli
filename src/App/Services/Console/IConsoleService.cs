using App.Services.Oracle;
using App.Validators;
using Spectre.Console;

namespace App.Services.Console;

public interface IConsoleService
{
    void RenderTitle(string text);
    void RenderVersion(string version);
    void RenderProblem(string text);
    void RenderText(string text, Color color);
    void RenderSettingsFile(string filepath);
    void RenderException(Exception exception);
    Task RenderStatusAsync(Func<Task> action);
    Task<T> RenderStatusAsync<T>(Func<Task<T>> func);
    bool GetYesOrNoAnswer(string text, bool defaultAnswer);
    void RenderValidationErrors(ValidationErrors validationErrors);
    Task CopyTextToClipboardAsync(string text, CancellationToken cancellationToken);
    void RenderOracleTable(OracleTable oracleTable, OracleParameters parameters);
    void RenderOracleTables(ICollection<OracleTable> oracleTables, OracleParameters parameters);
    void RenderOracleObjects(ICollection<OracleObject> oracleObjects, OracleParameters parameters);
    void RenderOracleSchemas(ICollection<OracleSchema> oracleSchemas, OracleParameters parameters);
    void RenderOracleSources(ICollection<OracleSource> oracleSources, OracleParameters parameters);
    void RenderOraclePackages(ICollection<OraclePackage> oraclePackages, OracleParameters parameters);
    void RenderOracleArguments(ICollection<OracleArgument> oracleArguments, OracleParameters parameters);
    void RenderOracleFunctions(ICollection<OracleFunction> oracleFunctions, OracleParameters parameters);
    void RenderOracleProcedures(ICollection<OracleProcedure> oracleProcedures, OracleParameters parameters);
}