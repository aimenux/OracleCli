using System.Reflection;
using App.Commands;

namespace App.Configuration;

public sealed class Settings
{
    public static class Cli
    {
        public const string UsageName = @"OracleCli";
        public const string FriendlyName = @"OracleCli";
        public const string Description = @"A net global tool helping to retrieve package(s), function(s), procedure(s) and argument(s) infos from oracle.";
        public static readonly string Version = GetInformationalVersion().Split("+").FirstOrDefault();
        
        private static string GetInformationalVersion()
        {
            return typeof(ToolCommand)
                .Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;
        }
    }
    
    public static class ExitCode
    {
        public const int Ok = 0;
        public const int Ko = -1;
    }

    public const int DatabaseMaxItems = 30;

    public const int DatabaseTimeoutInSeconds = 300;
    
    public string DefaultDatabaseToUse { get; init; }
    public string DefaultSchemaToUse { get; init; }
    public ICollection<Database> Databases { get; init; }
}