using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

[Command("Procedure", "Procedures", FullName = "List oracle procedures", Description = "List oracle procedures.")]
public class ProceduresCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;

    public ProceduresCommand(
        IConsoleService consoleService,
        IOracleService oracleService,
        IOptions<Settings> options) : base(
        consoleService,
        options)
    {
        _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
        DatabaseName = Settings.DefaultDatabaseToUse;
        OwnerName = Settings.DefaultSchemaToUse;
    }
    
    [Option("-d|--db", "Database name", CommandOptionType.SingleValue)]
    public string DatabaseName { get; init; }
    
    [Option("-o|--owner", "Owner/Schema name", CommandOptionType.SingleValue)]
    public string OwnerName { get; init; }

    [Option("-f|--filter", "Filter keyword", CommandOptionType.SingleValue)]
    public string FilterKeyword { get; init; }
    
    [Option("-m|--max", "Max items", CommandOptionType.SingleValue)]
    public int MaxItems { get; init; } = Settings.DatabaseMaxItems;

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
            var oracleProcedures = await _oracleService.GetOracleProceduresAsync(parameters, cancellationToken);
            ConsoleService.RenderOracleProcedures(oracleProcedures, parameters);
        });
    }
}