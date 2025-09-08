namespace API.DTOs;

/// <summary>
/// API-specific error response DTO
/// Stays in API layer as it's presentation-specific
/// </summary>
public class ErrorResponseDto
{
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}
