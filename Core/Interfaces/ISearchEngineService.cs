namespace Core.Interfaces;

/// <summary>
/// Interface for search engines - follows Open/Closed Principle
/// </summary>
public interface ISearchEngineService
{
    Task<IEnumerable<string>> SearchAsync(string searchTerm, int maxResults = 100);
}