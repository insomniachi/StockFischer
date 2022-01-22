namespace Lichess;

public class Parameters
{
    public override string ToString() => string.Join("&", GetType()
        .GetProperties()
        .Where(x => x.GetValue(this) is { })
        .Select(x => $"{x.Name.ToCamelCase()}={x.GetValue(this)}"));

    public string ToQueryParameters() => ToString();

    public IEnumerable<KeyValuePair<string, string>> ToPostContent() => GetType()
            .GetProperties()
            .Where(x => x.GetValue(this) is { })
            .Select(x => new KeyValuePair<string, string>(x.Name.ToCamelCase(), x.GetValue(this).ToString().ToCamelCase()));
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
