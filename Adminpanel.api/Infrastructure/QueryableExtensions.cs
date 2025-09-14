using System.Linq.Expressions;

public static class QueryableExtensions
{
    public static IQueryable<T> OrderBySafe<T>(this IQueryable<T> q, string? sortBy, string? sortDir,
        params (string key, Expression<Func<T, object>> expr)[] whitelist)
    {
        var entry = whitelist.FirstOrDefault(w => w.key.Equals(sortBy ?? "", StringComparison.OrdinalIgnoreCase));
        var expr = entry.expr ?? whitelist.First().expr;  // default to first
        var descending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
        return descending ? q.OrderByDescending(expr) : q.OrderBy(expr);
    }
}