namespace App.Configuration;

public sealed class Settings
{
    public static class Cli
    {
        public const string UsageName = @"Oracle";
        public const string FriendlyName = @"OracleCli";
    }
    
    public static class ExitCode
    {
        public const int Ok = 0;
        public const int Ko = -1;
    }

    public const int DatabaseMaxItems = 30;

    public const int DatabaseTimeoutInSeconds = 300;
    public ICollection<Database> Databases { get; init; }
}