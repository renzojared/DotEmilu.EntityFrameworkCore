namespace DotEmilu.EntityFrameworkCore.Extensions;

public static class QueryExtensions
{
    public static async Task<PaginatedList<T>> AsPaginatedListAsync<T>(this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
        where T : class
    {
        var count = await source.CountAsync(cancellationToken);

        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}