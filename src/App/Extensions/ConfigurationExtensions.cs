using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using static App.Extensions.PathExtensions;

namespace App.Extensions;

[ExcludeFromCodeCoverage]
public static class ConfigurationExtensions
{
    public static void AddJsonFile(this IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.SetBasePath(GetDirectoryPath());
        var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        configurationBuilder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
    }

    public static void AddUserSecrets(this IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddUserSecrets(typeof(Program).Assembly);
    }

    public static LogEventLevel GetDefaultLogLevel(this IConfiguration configuration)
    {
        return configuration.GetValue<LogEventLevel>("Serilog:MinimumLevel:Default");
    }

    public static string GetOutputTemplate(this IConfiguration configuration)
    {
        return configuration["Serilog:WriteTo:0:Args:outputTemplate"];
    }
}