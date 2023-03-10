using System.Text.RegularExpressions;

namespace App.Extensions;

public static class StringExtensions
{
    private static readonly Regex ExtraSpaceRegex = new Regex(@"\s\s+", RegexOptions.Compiled);
    
    private static readonly Regex LineBreaksRegex = new Regex(@"\r\n?|\n", RegexOptions.Compiled);
    
    public static bool IgnoreEquals(this string left, string right)
    {
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }
    
    public static bool IgnoreContains(this string left, string right)
    {
        return left is not null && left.Contains(right, StringComparison.OrdinalIgnoreCase);
    }
    
    public static bool IgnoreContains(this string[] left, string right)
    {
        return left is not null && left.Any(x => x.IgnoreContains(right));
    }
    
    public static string RemoveExtraSpaces(this string input)
    {
        return string.IsNullOrWhiteSpace(input) 
            ? string.Empty 
            : ExtraSpaceRegex.Replace(input, " ").Trim();
    }

    public static string RemoveLineBreaks(this string input)
    {
        return string.IsNullOrWhiteSpace(input) 
            ? string.Empty 
            : LineBreaksRegex.Replace(input, " ").Trim();
    }
}