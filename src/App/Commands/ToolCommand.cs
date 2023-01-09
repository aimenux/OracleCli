using System.Reflection;
using App.Configuration;
using App.Services.Console;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;
using static App.Extensions.PathExtensions;

namespace App.Commands;

[Command(Name = Settings.Cli.UsageName, FullName = $"\n{Settings.Cli.FriendlyName}", Description = Settings.Cli.Description)]
[Subcommand(typeof(ObjectsCommand), typeof(SchemasCommand), typeof(PackagesCommand), typeof(ProceduresCommand), typeof(FunctionsCommand), typeof(ArgumentsCommand))]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
public class ToolCommand : AbstractCommand
{
    public ToolCommand(IConsoleService consoleService, IOptions<Settings> options) : base(consoleService, options)
    {
    }

    [Option("-s|--settings", "Show settings information.", CommandOptionType.NoValue)]
    public bool ShowSettings { get; init; }

    protected override Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        if (ShowSettings)
        {
            var filepath = GetSettingFilePath();
            ConsoleService.RenderSettingsFile(filepath);
        }
        else
        {
            ConsoleService.RenderTitle(Settings.Cli.FriendlyName);
            app.ShowHelp();
        }

        return Task.CompletedTask;
    }

    private static string GetVersion()
    {
        var version = typeof(ToolCommand)
            .Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        
        return $"V{version}";
    }
}