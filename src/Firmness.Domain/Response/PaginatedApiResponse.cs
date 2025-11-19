namespace Firmness.Domain.Response;
// Paginated ApiResponse
public class PaginatedApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public IEnumerable<T> Data { get; set; } = Array.Empty<T>();
    public PaginationMetadata Pagination { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static PaginatedApiResponse<T> SuccessResponse(
        IEnumerable<T> data,
        int page,
        int pageSize,
        long totalItems,
        string? message = null)
    {
        return new PaginatedApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Pagination = new PaginationMetadata
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                HasPrevious = page > 1,
                HasNext = page < (int)Math.Ceiling((double)totalItems / pageSize)
            }
        };
    }

    // ErrorResponse
    public static PaginatedApiResponse<T> ErrorResponse(string message, string? errorCode = null)
    {
        return new PaginatedApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }
}

// PaginationMetadata
public class PaginationMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}

