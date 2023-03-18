using System.Diagnostics;
using App.Services.Console;
using App.Services.Oracle;
using App.Validators;
using Spectre.Console;

namespace Tests;

internal class FakeConsoleService : IConsoleService
{
    public void RenderTitle(string text)
    {
    }

    public void RenderVersion(string version)
    {
    }

    public void RenderProblem(string text)
    {
    }

    public void RenderText(string text, Color color)
    {
    }

    public void RenderSettingsFile(string filepath)
    {
    }
    
    public void RenderUserSecretsFile(string filepath)
    {
    }

    public void RenderException(Exception exception)
    {
        Debug.WriteLine(exception);
    }

    public async Task RenderStatusAsync(Func<Task> action)
    {
        await action.Invoke();
    }

    public async Task<T> RenderStatusAsync<T>(Func<Task<T>> func)
    {
        return await func.Invoke();
    }
    
    public bool GetYesOrNoAnswer(string text, bool defaultAnswer)
    {
        return true;
    }

    public void RenderValidationErrors(ValidationErrors validationErrors)
    {
    }

    public Task CopyTextToClipboardAsync(string text, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void RenderOracleInfo(OracleInfo oracleInfo, OracleArgs oracleArgs)
    {
    }

    public void RenderOracleTable(OracleTable oracleTable, OracleArgs oracleArgs)
    {
    }

    public void RenderOracleTables(ICollection<OracleTable> oracleTables, OracleArgs oracleArgs)
    {
    }

    public void RenderOracleObjects(ICollection<OracleObject> oracleObjects, OracleArgs oracleArgs)
    {
    }

    public void RenderOracleSchemas(ICollection<OracleSchema> oracleSchemas, OracleArgs oracleArgs)
    {
    }

    public void RenderOracleSources(ICollection<OracleSource> oracleSources, OracleArgs oracleArgs)
    {
    }

    public void RenderOraclePackages(ICollection<OraclePackage> oraclePackages, OracleArgs oracleArgs)
    {
    }

    public void RenderOracleParameters(ICollection<OracleParameter> oracleArguments, OracleArgs oracleArgs)
    {
    }

    public void RenderOracleFunctions(ICollection<OracleFunction> oracleFunctions, OracleArgs oracleArgs)
    {
    }

    public void RenderOracleProcedures(ICollection<OracleProcedure> oracleProcedures, OracleArgs oracleArgs)
    {
    }

    public void RenderOracleLocks(ICollection<OracleLock> oracleLocks, OracleArgs oracleArgs)
    {
    }

    public void RenderOracleSessions(ICollection<OracleSession> oracleSessions, OracleArgs oracleArgs)
    {
    }
}