using App.Services.Console;
using App.Services.Oracle;
using McMaster.Extensions.CommandLineUtils;

namespace App.Commands;

[Command("Argument", "Arguments", FullName = "List oracle arguments", Description = "List oracle arguments.")]
public class ArgumentsCommand : AbstractCommand
{
    private readonly IOracleService _oracleService;

    public ArgumentsCommand(IConsoleService consoleService, IOracleService oracleService) : base(consoleService)
    {
        _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    }

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
            var oracleArguments = await _oracleService.GetOracleArgumentsAsync(parameters, cancellationToken);
            ConsoleService.RenderOracleArguments(oracleArguments, parameters);
        });
    }
}