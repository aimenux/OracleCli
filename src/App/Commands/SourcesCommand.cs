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
        SchemaName = Settings.DefaultSchemaToUse;
    }

    [Option("-d|--db", "Database name", CommandOptionType.SingleValue)]
    public string DatabaseName { get; init; }

    [Option("-u|--schema", "Schema/User name", CommandOptionType.SingleValue)]
    public string SchemaName { get; init; }

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
        
        var oracleArgs = new OracleArgs
        {
            DatabaseName = DatabaseName,
            SchemaName = SchemaName,
            PackageName = PackageName,
            ProcedureName = ProcedureName,
            FunctionName = FunctionName,
            OutputDirectory = OutputDirectory,
            OutputFile = outputFile
        };
        
        var tasks = new List<Task>
        {
            ExecuteProcedureSourcesAsync(oracleArgs, cancellationToken),
            ExecuteFunctionSourcesAsync(oracleArgs, cancellationToken)
        };

        await Task.WhenAll(tasks);
    }
    
    private async Task ExecuteProcedureSourcesAsync(OracleArgs args, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(args.ProcedureName)) return;
        
        var oracleProcedures = await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleProcedures = await _oracleService.GetOracleProceduresAsync(args, cancellationToken);
            return oracleProcedures;
        });
        
        if (oracleProcedures.Count is 0 or > 1)
        {
            ConsoleService.RenderProblem($"Found {oracleProcedures.Count} procedure(s) matching name '{args.ProcedureName}'");
            if (oracleProcedures.Count > 1 && ConsoleService.GetYesOrNoAnswer("display found procedures on console screen", true))
            {
                ConsoleService.RenderOracleProcedures(oracleProcedures, args);
            }
        }
        else
        {
            await ConsoleService.RenderStatusAsync(async () =>
            {
                var oracleProcedure = oracleProcedures.Single();
                args = args.WithProcedure(oracleProcedure.SchemaName, oracleProcedure.PackageName, oracleProcedure.ProcedureName);
                var oracleSources = await _oracleService.GetOracleSourcesAsync(args, cancellationToken);
                await _exportService.ExportOracleSourcesAsync(oracleSources, args, cancellationToken);
                ConsoleService.RenderOracleSources(oracleSources, args);
            });
        }
    }
    
    private async Task ExecuteFunctionSourcesAsync(OracleArgs args, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(args.FunctionName)) return;
        
        var oracleFunctions = await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleFunctions = await _oracleService.GetOracleFunctionsAsync(args, cancellationToken);
            return oracleFunctions;
        });
        
        if (oracleFunctions.Count is 0 or > 1)
        {
            ConsoleService.RenderProblem($"Found {oracleFunctions.Count} function(s) matching name '{args.FunctionName}'");
            if (oracleFunctions.Count > 1 && ConsoleService.GetYesOrNoAnswer("display found functions on console screen", true))
            {
                ConsoleService.RenderOracleFunctions(oracleFunctions, args);
            }
        }
        else
        {
            await ConsoleService.RenderStatusAsync(async () =>
            {
                var oracleFunction = oracleFunctions.Single();
                args = args.WithFunction(oracleFunction.SchemaName, oracleFunction.PackageName, oracleFunction.FunctionName);
                var oracleSources = await _oracleService.GetOracleSourcesAsync(args, cancellationToken);
                await _exportService.ExportOracleSourcesAsync(oracleSources, args, cancellationToken);
                ConsoleService.RenderOracleSources(oracleSources, args);
            });
        }
    }
}