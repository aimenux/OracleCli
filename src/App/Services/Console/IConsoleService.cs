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
    void RenderOracleInfo(OracleInfo oracleInfo, OracleArgs oracleArgs);
    void RenderOracleTable(OracleTable oracleTable, OracleArgs oracleArgs);
    void RenderOracleTables(ICollection<OracleTable> oracleTables, OracleArgs oracleArgs);
    void RenderOracleObjects(ICollection<OracleObject> oracleObjects, OracleArgs oracleArgs);
    void RenderOracleSchemas(ICollection<OracleSchema> oracleSchemas, OracleArgs oracleArgs);
    void RenderOracleSources(ICollection<OracleSource> oracleSources, OracleArgs oracleArgs);
    void RenderOraclePackages(ICollection<OraclePackage> oraclePackages, OracleArgs oracleArgs);
    void RenderOracleParameters(ICollection<OracleParameter> oracleParameters, OracleArgs oracleArgs);
    void RenderOracleFunctions(ICollection<OracleFunction> oracleFunctions, OracleArgs oracleArgs);
    void RenderOracleProcedures(ICollection<OracleProcedure> oracleProcedures, OracleArgs oracleArgs);
    void RenderOracleLocks(ICollection<OracleLock> oracleLocks, OracleArgs oracleArgs);
    void RenderOracleSessions(ICollection<OracleSession> oracleSessions, OracleArgs oracleArgs);
}