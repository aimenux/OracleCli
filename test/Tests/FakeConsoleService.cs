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

    public void RenderOracleInfo(OracleInfo oracleInfo, OracleParameters parameters)
    {
    }

    public void RenderOracleTable(OracleTable oracleTable, OracleParameters parameters)
    {
    }

    public void RenderOracleTables(ICollection<OracleTable> oracleTables, OracleParameters parameters)
    {
    }

    public void RenderOracleObjects(ICollection<OracleObject> oracleObjects, OracleParameters parameters)
    {
    }

    public void RenderOracleSchemas(ICollection<OracleSchema> oracleSchemas, OracleParameters parameters)
    {
    }

    public void RenderOracleSources(ICollection<OracleSource> oracleSources, OracleParameters parameters)
    {
    }

    public void RenderOraclePackages(ICollection<OraclePackage> oraclePackages, OracleParameters parameters)
    {
    }

    public void RenderOracleArguments(ICollection<OracleArgument> oracleArguments, OracleParameters parameters)
    {
    }

    public void RenderOracleFunctions(ICollection<OracleFunction> oracleFunctions, OracleParameters parameters)
    {
    }

    public void RenderOracleProcedures(ICollection<OracleProcedure> oracleProcedures, OracleParameters parameters)
    {
    }

    public void RenderOracleLocks(ICollection<OracleLock> oracleLocks, OracleParameters parameters)
    {
    }

    public void RenderOracleSessions(ICollection<OracleSession> oracleSessions, OracleParameters parameters)
    {
    }
}