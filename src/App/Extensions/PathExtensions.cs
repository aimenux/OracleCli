using System.Reflection;

namespace App.Extensions;

public static class PathExtensions
{
    public static string GetSettingFilePath() => Path.GetFullPath(Path.Combine(GetDirectoryPath(), @"appsettings.json"));

    public static string GetDirectoryPath()
    {
        try
        {
            return Path.GetDirectoryName(GetAssemblyLocation());
        }
        catch
        {
            return Directory.GetCurrentDirectory();
        }
    }

    public static string GetAssemblyLocation() => Assembly.GetExecutingAssembly().Location;
}