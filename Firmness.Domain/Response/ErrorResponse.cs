namespace Firmness.Domain.Response;


// Response error
public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }

    public static ErrorResponse Create(string message, string? errorCode = null)
    {
        return new ErrorResponse
        {
            Message = message,
            ErrorCode = errorCode
        };
    }

    public static ErrorResponse CreateValidationError(
        string message,
        Dictionary<string, string[]> validationErrors)
    {
        return new ErrorResponse
        {
            Message = message,
            ErrorCode = "VALIDATION_ERROR",
            ValidationErrors = validationErrors
        };
    }
}
