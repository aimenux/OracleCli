using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

[Command("Schema", "Schemas", "Owner", "Owners", FullName = "List oracle schema", Description = "List oracle schemas.")]
public class SchemasCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;
    
    public SchemasCommand(
        IConsoleService consoleService,
        IOracleService oracleService,
        IOptions<Settings> options) : base(
        consoleService,
        options)
    {
        _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
        DatabaseName = Settings.DefaultDatabaseToUse;
    }
    
    [Option("-d|--db", "Database name", CommandOptionType.SingleValue)]
    public string DatabaseName { get; init; }
        
    [Option("-n|--name", "Schema name", CommandOptionType.SingleValue)]
    public string SchemaName { get; init; }
    
    [Option("--filter", "Filter keyword", CommandOptionType.SingleValue)]
    public string FilterKeyword { get; init; }
    
    [Option("-m|--max", "Max items", CommandOptionType.SingleValue)]
    public int MaxItems { get; init; } = Settings.DatabaseMaxItems;

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var oracleArgs = new OracleArgs
        {
            DatabaseName = DatabaseName,
            OwnerName = SchemaName,
            MaxItems = MaxItems,
            FilterKeyword = FilterKeyword
        };

        await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleSchemas = await _oracleService.GetOracleSchemasAsync(oracleArgs, cancellationToken);
            ConsoleService.RenderOracleSchemas(oracleSchemas, oracleArgs);
        });
    }
}