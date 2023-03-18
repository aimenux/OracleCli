using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

[Command("Object", "Objects", FullName = "List oracle objects", Description = "List oracle objects.")]
public class ObjectsCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;

    public ObjectsCommand(
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

    [Option("-t|--type", "Object type(s)", CommandOptionType.MultipleValue)]
    public string[] ObjectTypes { get; init; }

    [Option("--filter", "Filter keyword", CommandOptionType.SingleValue)]
    public string FilterKeyword { get; init; }
    
    [Option("-m|--max", "Max items", CommandOptionType.SingleValue)]
    public int MaxItems { get; init; } = Settings.DatabaseMaxItems;

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var oracleArgs = new OracleArgs
        {
            DatabaseName = DatabaseName,
            SchemaName = SchemaName,
            MaxItems = MaxItems,
            ObjectTypes = ObjectTypes,
            FilterKeyword = FilterKeyword
        };

        await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleObjects = await _oracleService.GetOracleObjectsAsync(oracleArgs, cancellationToken);
            ConsoleService.RenderOracleObjects(oracleObjects, oracleArgs);
        });
    }
}