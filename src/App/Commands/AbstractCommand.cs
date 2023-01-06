using App.Configuration;
using App.Services.Console;
using App.Validators;
using McMaster.Extensions.CommandLineUtils;

namespace App.Commands;

public abstract class AbstractCommand
{
    protected readonly IConsoleService ConsoleService;

    protected AbstractCommand(IConsoleService consoleService)
    {
        ConsoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
    }
    
    [Option("-d|--db", "Database name", CommandOptionType.SingleValue)]
    public string DatabaseName { get; init; }
    
    [Option("-o|--owner", "Owner/Schema name", CommandOptionType.SingleValue)]
    public string OwnerName { get; init; }

    public async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!HasValidOptionsAndArguments(out var validationErrors))
            {
                ConsoleService.RenderValidationErrors(validationErrors);
                return Settings.ExitCode.Ko;
            }

            await ExecuteAsync(app, cancellationToken);
            return Settings.ExitCode.Ok;
        }
        catch (Exception ex)
        {
            ConsoleService.RenderException(ex);
            return Settings.ExitCode.Ko;
        }
    }

    protected abstract Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default);

    protected virtual bool HasValidOptionsAndArguments(out ValidationErrors validationErrors)
    {
        validationErrors = ToolCommandValidator.Validate(this);
        return !validationErrors.Any();
    }
}