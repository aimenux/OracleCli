using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

[Command("Function", "Functions", FullName = "List oracle functions", Description = "List oracle functions.")]
public class FunctionsCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;

    public FunctionsCommand(
        IConsoleService consoleService,
        IOracleService oracleService,
        IOptions<Settings> options) : base(
        consoleService,
        options)
    {
        _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
        DatabaseName = Settings.DefaultDatabaseToUse;
        SchemaName = Settings.DefaultSchemaToUse;
    }
    
    [Option("-d|--db", "Database name", CommandOptionType.SingleValue)]
    public string DatabaseName { get; init; }
    
    [Option("-u|--schema", "Schema/User name", CommandOptionType.SingleValue)]
    public string SchemaName { get; init; }
    
    [Option("-p|--pkg", "Package name", CommandOptionType.SingleValue)]
    public string PackageName { get; init; }
    
    [Option("-n|--name", "Function name", CommandOptionType.SingleValue)]
    public string FunctionName { get; init; }

    [Option("--filter", "Filter keyword", CommandOptionType.SingleValue)]
    public string FilterKeyword { get; init; }
    
    [Option("-m|--max", "Max items", CommandOptionType.SingleValue)]
    public int MaxItems { get; init; } = Settings.DatabaseMaxItems;

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var oracleArgs = new OracleArgs
        {
            DatabaseName = DatabaseName,
            PackageName = PackageName,
            FunctionName = FunctionName,
            SchemaName = SchemaName,
            MaxItems = MaxItems,
            FilterKeyword = FilterKeyword
        };

        await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleFunctions = await _oracleService.GetOracleFunctionsAsync(oracleArgs, cancellationToken);
            ConsoleService.RenderOracleFunctions(oracleFunctions, oracleArgs);
        });
    }
}