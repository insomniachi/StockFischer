namespace Lichess;

public class QueryParameters
{
    public override string ToString() => string.Join("&", GetType()
        .GetProperties()
        .Where(x => x.GetValue(this) is { })
        .Select(x => $"{x.Name.ToCamelCase()}={x.GetValue(this)}"));
}

public static class StringExtension
{
    public static string ToCamelCase(this string str)
    {
        if (!string.IsNullOrEmpty(str) && str.Length > 1)
        {
            return char.ToLowerInvariant(str[0]) + str[1..];
        }
        return str;
    }
}
