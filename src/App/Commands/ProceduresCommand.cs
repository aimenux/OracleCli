using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;

namespace App.Commands;

[Command("Procedure", "Procedures", FullName = "List oracle procedures", Description = "List oracle procedures.")]
public class ProceduresCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;

    public ProceduresCommand(IConsoleService consoleService, IOracleService oracleService) : base(consoleService)
    {
        _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    }
    
    [Option("-m|--max", "Max items", CommandOptionType.SingleValue)]
    public int MaxItems { get; init; } = Settings.DatabaseMaxItems;
    
    [Option("-k|--keyword", "Keyword search", CommandOptionType.SingleValue)]
    public string Keyword { get; init; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var parameters = new OracleParameters
        {
            DatabaseName = DatabaseName,
            OwnerName = OwnerName,
            MaxItems = MaxItems,
            Keyword = Keyword
        };

        await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleProcedures = await _oracleService.GetOracleProceduresAsync(parameters, cancellationToken);
            ConsoleService.RenderOracleProcedures(oracleProcedures, parameters);
        });
    }
}