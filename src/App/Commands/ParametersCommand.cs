using App.Configuration;
using App.Services.Console;
using App.Services.Exporters;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

[Command("Parameter", "Parameters", FullName = "List oracle procedure/function parameters", Description = "List oracle procedure/function parameters.")]
public class ParametersCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;
    private readonly ICSharpExportService _exportService;

    public ParametersCommand(
        IConsoleService consoleService,
        IOracleService oracleService,
        ICSharpExportService exportService,
        IOptions<Settings> options) : base(
        consoleService,
        options)
    {
        _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
        _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
        DatabaseName = Settings.DefaultDatabaseToUse;
        OwnerName = Settings.DefaultSchemaToUse;
    }

    [Option("-d|--db", "Database name", CommandOptionType.SingleValue)]
    public string DatabaseName { get; init; }

    [Option("-o|--owner", "Owner/Schema name", CommandOptionType.SingleValue)]
    public string OwnerName { get; init; }

    [Option("-p|--pkg", "Package name", CommandOptionType.SingleValue)]
    public string PackageName { get; init; }

    [Option("-s|--spc", "Procedure name", CommandOptionType.SingleValue)]
    public string ProcedureName { get; init; }
    
    [Option("-f|--fun", "Function name", CommandOptionType.SingleValue)]
    public string FunctionName { get; init; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var oracleArgs = new OracleArgs
        {
            DatabaseName = DatabaseName,
            OwnerName = OwnerName,
            PackageName = PackageName,
            ProcedureName = ProcedureName,
            FunctionName = FunctionName
        };

        var tasks = new List<Task>
        {
            ExecuteProcedureParametersAsync(oracleArgs, cancellationToken),
            ExecuteFunctionParametersAsync(oracleArgs, cancellationToken)
        };

        await Task.WhenAll(tasks);
    }
    
    private async Task ExecuteProcedureParametersAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(oracleArgs.ProcedureName)) return;
        
        var oracleProcedures = await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleProcedures = await _oracleService.GetOracleProceduresAsync(oracleArgs, cancellationToken);
            return oracleProcedures;
        });
        
        if (oracleProcedures.Count is 0 or > 1)
        {
            ConsoleService.RenderProblem($"Found {oracleProcedures.Count} procedure(s) matching name '{oracleArgs.ProcedureName}'");
            if (oracleProcedures.Count > 1 && ConsoleService.GetYesOrNoAnswer("display found procedures on console screen", true))
            {
                ConsoleService.RenderOracleProcedures(oracleProcedures, oracleArgs);
            }
        }
        else
        {
            await ConsoleService.RenderStatusAsync(async () =>
            {
                var oracleProcedure = oracleProcedures.Single();
                oracleArgs = oracleArgs.WithProcedure(oracleProcedure.OwnerName, oracleProcedure.PackageName, oracleProcedure.ProcedureName);
                var oracleParameters = await _oracleService.GetOracleParametersAsync(oracleArgs, cancellationToken);
                await _exportService.ExportOracleParametersAsync(oracleParameters, oracleArgs, cancellationToken);
                ConsoleService.RenderOracleParameters(oracleParameters, oracleArgs);
            });
        }
    }
    
    private async Task ExecuteFunctionParametersAsync(OracleArgs oracleArgs, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(oracleArgs.FunctionName)) return;
        
        var oracleFunctions = await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleFunctions = await _oracleService.GetOracleFunctionsAsync(oracleArgs, cancellationToken);
            return oracleFunctions;
        });
        
        if (oracleFunctions.Count is 0 or > 1)
        {
            ConsoleService.RenderProblem($"Found {oracleFunctions.Count} function(s) matching name '{oracleArgs.FunctionName}'");
            if (oracleFunctions.Count > 1 && ConsoleService.GetYesOrNoAnswer("display found functions on console screen", true))
            {
                ConsoleService.RenderOracleFunctions(oracleFunctions, oracleArgs);
            }
        }
        else
        {
            await ConsoleService.RenderStatusAsync(async () =>
            {
                var oracleFunction = oracleFunctions.Single();
                oracleArgs = oracleArgs.WithFunction(oracleFunction.OwnerName, oracleFunction.PackageName, oracleFunction.FunctionName);
                var oracleParameters = await _oracleService.GetOracleParametersAsync(oracleArgs, cancellationToken);
                await _exportService.ExportOracleParametersAsync(oracleParameters, oracleArgs, cancellationToken);
                ConsoleService.RenderOracleParameters(oracleParameters, oracleArgs);
            });
        }
    }
}