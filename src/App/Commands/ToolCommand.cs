using System.Reflection;
using App.Configuration;
using App.Services.Console;
using McMaster.Extensions.CommandLineUtils;
using static App.Extensions.PathExtensions;

namespace App.Commands;

[Command(Name = Settings.Cli.UsageName, FullName = $"\n{Settings.Cli.FriendlyName}", Description = "A net global tool helping to retrieve package(s), function(s), procedure(s) and argument(s) infos from oracle.")]
[Subcommand(typeof(ObjectsCommand), typeof(PackagesCommand), typeof(ProceduresCommand), typeof(FunctionsCommand), typeof(ArgumentsCommand))]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
public class ToolCommand : AbstractCommand
{
    public ToolCommand(IConsoleService consoleService) : base(consoleService)
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