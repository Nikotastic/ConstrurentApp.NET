namespace Firmness.Core.Common;

public interface IPaginatedResult<T>
{
    IEnumerable<T> Items { get; }
    int Page { get; }
    int PageSize { get; }
    long Total { get; }
    int TotalPages { get; }
    bool HasPrevious { get; }
    bool HasNext { get; }
}