using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;

namespace App.Commands;

[Command("Function", "Functions", FullName = "List oracle functions", Description = "List oracle functions.")]
public class FunctionsCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;

    public FunctionsCommand(IConsoleService consoleService, IOracleService oracleService) : base(consoleService)
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
            var oracleFunctions = await _oracleService.GetOracleFunctionsAsync(parameters, cancellationToken);
            ConsoleService.RenderOracleFunctions(oracleFunctions, parameters);
        });
    }
}