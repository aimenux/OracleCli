namespace App.Extensions;

public static class StringExtensions
{
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
}