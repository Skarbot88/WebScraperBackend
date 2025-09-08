namespace Core.DTOs;

// <summary>
/// Request DTO for search operations
/// </summary>
public class SearchRequestDto
{
    public string SearchTerm { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
}