using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;

namespace App.Commands;

[Command("Object", "Objects", FullName = "List oracle objects", Description = "List oracle objects.")]
public class ObjectsCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;

    public ObjectsCommand(IConsoleService consoleService, IOracleService oracleService) : base(consoleService)
    {
        _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    }
    
    [Option("-m|--max", "Max items", CommandOptionType.SingleValue)]
    public int MaxItems { get; init; } = Settings.DatabaseMaxItems;
    
    [Option("-f|--filter", "Filter keyword", CommandOptionType.SingleValue)]
    public string FilterKeyword { get; init; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var parameters = new OracleParameters
        {
            DatabaseName = DatabaseName,
            OwnerName = OwnerName,
            MaxItems = MaxItems,
            FilterKeyword = FilterKeyword
        };

        await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleObjects = await _oracleService.GetOracleObjectsAsync(parameters, cancellationToken);
            ConsoleService.RenderOracleObjects(oracleObjects, parameters);
        });
    }
}