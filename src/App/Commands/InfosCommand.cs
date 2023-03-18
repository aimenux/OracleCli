using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

[Command("Info", "Infos", FullName = "Get oracle infos", Description = "Get oracle infos.")]
public class InfosCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;
    
    public InfosCommand(
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

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var parameters = new OracleParameters
        {
            DatabaseName = DatabaseName
        };

        await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleInfo = await _oracleService.GetOracleInfoAsync(parameters, cancellationToken);
            ConsoleService.RenderOracleInfo(oracleInfo, parameters);
        });
    }
}