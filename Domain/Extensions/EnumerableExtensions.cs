namespace Domain.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
    {
        return source.Where(item => item is not null)!;
    }
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : struct
    {
        return source.Where(item => item.HasValue).Select(x => x!.Value);
    }

}
