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
    void RenderUserSecretsFile(string filepath);
    void RenderException(Exception exception);
    Task RenderStatusAsync(Func<Task> action);
    Task<T> RenderStatusAsync<T>(Func<Task<T>> func);
    bool GetYesOrNoAnswer(string text, bool defaultAnswer);
    void RenderValidationErrors(ValidationErrors validationErrors);
    Task CopyTextToClipboardAsync(string text, CancellationToken cancellationToken);
    void RenderOracleInfo(OracleInfo oracleInfo, OracleArgs args);
    void RenderOracleTable(OracleTable oracleTable, OracleArgs args);
    void RenderOracleTables(ICollection<OracleTable> oracleTables, OracleArgs args);
    void RenderOracleObjects(ICollection<OracleObject> oracleObjects, OracleArgs args);
    void RenderOracleSchemas(ICollection<OracleSchema> oracleSchemas, OracleArgs args);
    void RenderOracleSources(ICollection<OracleSource> oracleSources, OracleArgs args);
    void RenderOraclePackages(ICollection<OraclePackage> oraclePackages, OracleArgs args);
    void RenderOracleArguments(ICollection<OracleArgument> oracleArguments, OracleArgs args);
    void RenderOracleFunctions(ICollection<OracleFunction> oracleFunctions, OracleArgs args);
    void RenderOracleProcedures(ICollection<OracleProcedure> oracleProcedures, OracleArgs args);
    void RenderOracleLocks(ICollection<OracleLock> oracleLocks, OracleArgs args);
    void RenderOracleSessions(ICollection<OracleSession> oracleSessions, OracleArgs args);
}