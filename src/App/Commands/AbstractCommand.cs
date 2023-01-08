using App.Configuration;
using App.Services.Console;
using App.Validators;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace App.Commands;

public abstract class AbstractCommand
{
    protected readonly IConsoleService ConsoleService;
    protected readonly Settings Settings;

    protected AbstractCommand(IConsoleService consoleService, IOptions<Settings> options)
    {
        ConsoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
        Settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

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