namespace Core.Interfaces;

/// <summary>
/// Interface for position analysis - Interface Segregation Principle
/// </summary>
public interface IPositionAnalyser
{
    IEnumerable<int> FindPositions(IEnumerable<string> searchResults, string targetUrl);
    bool IsUrlMatch(string searchResultUrl, string targetUrl);
}