using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

[Command("Lock", "Locks", FullName = "List oracle locks", Description = "List oracle locks.")]
public class LocksCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;
    
    public LocksCommand(
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

    [Option("-t|--time", "Minimum blocking time in minutes", CommandOptionType.SingleValue)]
    public int MinBlockingTimeInMinutes { get; init; } = 5;

    [Option("-m|--max", "Max items", CommandOptionType.SingleValue)]
    public int MaxItems { get; init; } = Settings.DatabaseMaxItems;

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var oracleArgs = new OracleArgs
        {
            DatabaseName = DatabaseName,
            SchemaName = SchemaName,
            MinBlockingTimeInMinutes = MinBlockingTimeInMinutes,
            MaxItems = MaxItems
        };

        await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleLocks = await _oracleService.GetOracleLocksAsync(oracleArgs, cancellationToken);
            ConsoleService.RenderOracleLocks(oracleLocks, oracleArgs);
        });
    }
}