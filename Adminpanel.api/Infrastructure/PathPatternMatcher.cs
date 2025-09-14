using System.Collections.Concurrent;
using System.Text.RegularExpressions;

public static class PathPatternMatcher
{
    private static readonly ConcurrentDictionary<string, Regex> Cache = new();

    public static bool IsMatch(string pattern, string path)
    {
        var rx = Cache.GetOrAdd(pattern, p =>
            new Regex("^" + Regex.Escape(p).Replace("\\*", ".*") + "$",
                      RegexOptions.IgnoreCase | RegexOptions.Compiled));
        return rx.IsMatch(path);
    }
}
