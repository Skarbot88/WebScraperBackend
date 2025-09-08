namespace Core.DTOs;

// <summary>
/// Response DTO for search results
/// </summary>
public class SearchResultDto
{
    public IEnumerable<int> Positions { get; set; } = new List<int>();
    public string SearchTerm { get; set; } = string.Empty;
    public int TotalHits
    {
        get
        {
            return this.Positions.Count();
        }
        
    }
    public string TargetUrl { get; set; } = string.Empty;
    public DateTime SearchDate { get; set; }
    public int TotalResults { get; set; }
    public string FormattedPositions => Positions.Any() ? string.Join(", ", Positions) : "0";
}