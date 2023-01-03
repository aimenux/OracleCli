using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;

namespace App.Commands;

[Command("Package", "Packages", FullName = "List oracle packages", Description = "List oracle packages.")]
public class PackagesCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;

    public PackagesCommand(IConsoleService consoleService, IOracleService oracleService) : base(consoleService)
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
            var oraclePackages = await _oracleService.GetOraclePackagesAsync(parameters, cancellationToken);
            ConsoleService.RenderOraclePackages(oraclePackages, parameters);
        });
    }
}