using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

[Command("Table", "Tables", FullName = "List oracle tables", Description = "List oracle tables.")]
public class TablesCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;

    public TablesCommand(
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
    
    [Option("-n|--name", "Table name", CommandOptionType.SingleValue)]
    public string TableName { get; init; }

    [Option("--filter", "Filter keyword", CommandOptionType.SingleValue)]
    public string FilterKeyword { get; init; }
    
    [Option("-m|--max", "Max items", CommandOptionType.SingleValue)]
    public int MaxItems { get; init; } = Settings.DatabaseMaxItems;

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var parameters = new OracleParameters
        {
            DatabaseName = DatabaseName,
            TableName = TableName,
            OwnerName = OwnerName,
            MaxItems = MaxItems,
            FilterKeyword = FilterKeyword
        };

        var oracleTables = await ConsoleService.RenderStatusAsync(async () =>
        {
            var results = await _oracleService.GetOracleTablesAsync(parameters, cancellationToken);
            ConsoleService.RenderOracleTables(results, parameters);
            return results;
        });

        if (oracleTables.Count == 1 && ConsoleService.GetYesOrNoAnswer("display table columns on console screen", true))
        {
            var oracleTable = oracleTables.Single();
            parameters = parameters.WithTable(oracleTable.OwnerName, oracleTable.TableName);
            oracleTable = await _oracleService.GetOracleTableAsync(parameters, cancellationToken);
            ConsoleService.RenderOracleTable(oracleTable, parameters);
        }
    }
}