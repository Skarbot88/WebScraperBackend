namespace API.DTOs;

/// <summary>
/// API-specific response wrapper
/// Provides consistent API response structure
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public ErrorResponseDto? Error { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, string? details = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = new ErrorResponseDto
            {
                Message = message,
                Details = details
            }
        };
    }
}