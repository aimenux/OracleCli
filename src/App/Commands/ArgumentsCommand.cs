using App.Configuration;
using App.Services.Console;
using App.Services.Exporters;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

[Command("Argument", "Arguments", FullName = "List oracle arguments", Description = "List oracle arguments.")]
public class ArgumentsCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;
    private readonly ICSharpExportService _exportService;

    public ArgumentsCommand(
        IConsoleService consoleService,
        IOracleService oracleService,
        ICSharpExportService exportService,
        IOptions<Settings> options) : base(
        consoleService,
        options)
    {
        _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
        _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
        DatabaseName = Settings.DefaultDatabaseToUse;
        OwnerName = Settings.DefaultSchemaToUse;
    }

    [Option("-d|--db", "Database name", CommandOptionType.SingleValue)]
    public string DatabaseName { get; init; }

    [Option("-o|--owner", "Owner/Schema name", CommandOptionType.SingleValue)]
    public string OwnerName { get; init; }

    [Option("-p|--pkg", "Package name", CommandOptionType.SingleValue)]
    public string PackageName { get; init; }

    [Option("-s|--spc", "Procedure name", CommandOptionType.SingleValue)]
    public string ProcedureName { get; init; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var parameters = new OracleParameters
        {
            DatabaseName = DatabaseName,
            OwnerName = OwnerName,
            PackageName = PackageName,
            ProcedureName = ProcedureName
        };

        var oracleProcedures = await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleProcedures = await _oracleService.GetOracleProceduresAsync(parameters, cancellationToken);
            return oracleProcedures;
        });
        
        if (oracleProcedures.Count is 0 or > 1)
        {
            ConsoleService.RenderFoundOracleProcedures(oracleProcedures, parameters);
        }
        else
        {
            await ConsoleService.RenderStatusAsync(async () =>
            {
                var oracleProcedure = oracleProcedures.Single();
                parameters = parameters.With(oracleProcedure.OwnerName, oracleProcedure.PackageName, oracleProcedure.ProcedureName);
                var oracleArguments = await _oracleService.GetOracleArgumentsAsync(parameters, cancellationToken);
                await _exportService.ExportOracleArgumentsAsync(oracleArguments, parameters, cancellationToken);
                ConsoleService.RenderOracleArguments(oracleArguments, parameters);
            });
        }
    }
}