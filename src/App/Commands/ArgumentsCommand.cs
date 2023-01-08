using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

[Command("Argument", "Arguments", FullName = "List oracle arguments", Description = "List oracle arguments.")]
public class ArgumentsCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;

    public ArgumentsCommand(
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

        await ConsoleService.RenderStatusAsync(async () =>
        {
            var oracleProcedures = await FindOracleProceduresAsync(parameters, cancellationToken);
            if (oracleProcedures.Count is 0 or > 1)
            {
                ConsoleService.RenderText($"Found {oracleProcedures.Count} procedure(s) matching name '{parameters.ProcedureName}'");
            }
            else
            {
                var oracleProcedure = oracleProcedures.Single();
                parameters = parameters.With(oracleProcedure.PackageName, oracleProcedure.ProcedureName);
                var oracleArguments = await _oracleService.GetOracleArgumentsAsync(parameters, cancellationToken);
                ConsoleService.RenderOracleArguments(oracleArguments, parameters);
                ConsoleService.CopyOracleArgumentsToClipboard(oracleArguments, parameters);
            }
        });
    }

    private async Task<ICollection<OracleProcedure>> FindOracleProceduresAsync(OracleParameters parameters, CancellationToken cancellationToken)
    {
        var oracleProcedures = await _oracleService.FindOracleProceduresAsync(parameters, cancellationToken);
        return oracleProcedures.ToList();
    }
}