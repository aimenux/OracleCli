using App.Configuration;
using App.Extensions;
using App.Services.Console;
using App.Services.Exporters;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

[Command("Source", "Sources", FullName = "Get oracle sources", Description = "Get oracle sources.")]
public class SourcesCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;
    private readonly ISqlExportService _exportService;

    public SourcesCommand(
        IConsoleService consoleService,
        IOracleService oracleService,
        ISqlExportService exportService,
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
    
    [Option("-out|--output", "Output directory", CommandOptionType.SingleValue)]
    public string OutputDirectory { get; init; } = Settings.GetDefaultWorkingDirectory();

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var outputFile = OutputDirectory.GenerateFileName(ProcedureName ?? FunctionName);
        
        var parameters = new OracleParameters
        {
            DatabaseName = DatabaseName,
            OwnerName = OwnerName,
            PackageName = PackageName,
            ProcedureName = ProcedureName,
            FunctionName = FunctionName,
            OutputDirectory = OutputDirectory,
            OutputFile = outputFile
        };
        
        var tasks = new List<Task>
        {
            ExecuteProcedureSourcesAsync(parameters, cancellationToken),
            ExecuteFunctionSourcesAsync(parameters, cancellationToken)
        };

        await Task.WhenAll(tasks);
    }
    
    private async Task ExecuteProcedureSourcesAsync(OracleParameters parameters, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parameters.ProcedureName)) return;
        
        var oracleProcedures = await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleProcedures = await _oracleService.GetOracleProceduresAsync(parameters, cancellationToken);
            return oracleProcedures;
        });
        
        if (oracleProcedures.Count is 0 or > 1)
        {
            ConsoleService.RenderProblem($"Found {oracleProcedures.Count} procedure(s) matching name '{parameters.ProcedureName}'");
            if (oracleProcedures.Count > 1 && ConsoleService.GetYesOrNoAnswer("display found procedures on console screen", true))
            {
                ConsoleService.RenderOracleProcedures(oracleProcedures, parameters);
            }
        }
        else
        {
            await ConsoleService.RenderStatusAsync(async () =>
            {
                var oracleProcedure = oracleProcedures.Single();
                parameters = parameters.WithProcedure(oracleProcedure.OwnerName, oracleProcedure.PackageName, oracleProcedure.ProcedureName);
                var oracleSources = await _oracleService.GetOracleSourcesAsync(parameters, cancellationToken);
                await _exportService.ExportOracleSourcesAsync(oracleSources, parameters, cancellationToken);
                ConsoleService.RenderOracleSources(oracleSources, parameters);
            });
        }
    }
    
    private async Task ExecuteFunctionSourcesAsync(OracleParameters parameters, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parameters.FunctionName)) return;
        
        var oracleFunctions = await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleFunctions = await _oracleService.GetOracleFunctionsAsync(parameters, cancellationToken);
            return oracleFunctions;
        });
        
        if (oracleFunctions.Count is 0 or > 1)
        {
            ConsoleService.RenderProblem($"Found {oracleFunctions.Count} function(s) matching name '{parameters.FunctionName}'");
            if (oracleFunctions.Count > 1 && ConsoleService.GetYesOrNoAnswer("display found functions on console screen", true))
            {
                ConsoleService.RenderOracleFunctions(oracleFunctions, parameters);
            }
        }
        else
        {
            await ConsoleService.RenderStatusAsync(async () =>
            {
                var oracleFunction = oracleFunctions.Single();
                parameters = parameters.WithFunction(oracleFunction.OwnerName, oracleFunction.PackageName, oracleFunction.FunctionName);
                var oracleSources = await _oracleService.GetOracleSourcesAsync(parameters, cancellationToken);
                await _exportService.ExportOracleSourcesAsync(oracleSources, parameters, cancellationToken);
                ConsoleService.RenderOracleSources(oracleSources, parameters);
            });
        }
    }
}