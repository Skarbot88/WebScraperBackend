namespace Core.DTOs;

/// <summary>
/// DTO for trend analysis data - moved to Core for clean architecture
/// </summary>
public class TrendDataDto
{
    public DateTime Date { get; set; }
    public int BestPosition { get; set; }
    public int TotalOccurrences { get; set; }
    public IEnumerable<int> Positions { get; set; } = new List<int>();
}