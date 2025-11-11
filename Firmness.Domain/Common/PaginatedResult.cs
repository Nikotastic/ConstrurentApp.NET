namespace Firmness.Domain.Common;

public class PaginatedResult<T> : IPaginatedResult<T>
{
    public IEnumerable<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public long TotalItems { get; init; }
    public long Total => TotalItems;
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);

    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public PaginatedResult() { }

   
    public PaginatedResult(IEnumerable<T> items, int page, int pageSize, long totalItems)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be >= 1.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be >= 1.");
        if (totalItems < 0) throw new ArgumentOutOfRangeException(nameof(totalItems), "TotalItems must be >= 0.");

        Items = items ?? Array.Empty<T>();
        Page = page;
        PageSize = pageSize;
        TotalItems = totalItems;
    }

    public static PaginatedResult<T> Empty(int page = 1, int pageSize = 10) =>
        new PaginatedResult<T>(Array.Empty<T>(), Math.Max(1, page), Math.Max(1, pageSize), 0);
}