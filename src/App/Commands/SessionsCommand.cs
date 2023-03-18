using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

[Command("Session", "Sessions", FullName = "List oracle sessions", Description = "List oracle sessions.")]
public class SessionsCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;
    
    public SessionsCommand(
        IConsoleService consoleService,
        IOracleService oracleService,
        IOptions<Settings> options) : base(consoleService,
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

    [Option("-m|--max", "Max items", CommandOptionType.SingleValue)]
    public int MaxItems { get; init; } = Settings.DatabaseMaxItems;

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var oracleArgs = new OracleArgs
        {
            DatabaseName = DatabaseName,
            SchemaName = SchemaName,
            MaxItems = MaxItems
        };

        await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleSessions = await _oracleService.GetOracleSessionsAsync(oracleArgs, cancellationToken);
            ConsoleService.RenderOracleSessions(oracleSessions, oracleArgs);
        });
    }
}