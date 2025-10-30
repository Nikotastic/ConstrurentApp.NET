namespace Firmness.Core.Common;

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public long TotalItems { get; init; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);

    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public PaginatedResult() { }

    public PaginatedResult(IEnumerable<T> items, int page, int pageSize, long totalItems)
    {
        Items = items ?? Array.Empty<T>();
        Page = page < 1 ? 1 : page;
        PageSize = pageSize < 1 ? 10 : pageSize;
        TotalItems = totalItems;
    }
}