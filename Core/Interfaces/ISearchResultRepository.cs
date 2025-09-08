using Core.Models;

namespace Core.Interfaces;

/// <summary>
/// Interface for search result repository - Dependency Inversion Principle
/// </summary>
public interface ISearchResultRepository
{
    Task<SearchResult> SaveSearchResultAsync(SearchResult searchResult);
    Task<IEnumerable<SearchResult>> GetSearchHistoryAsync(string? searchTerm = null, int days = 30);
    Task<IEnumerable<SearchResult>> GetSearchResultsByTermAsync(string searchTerm, int days = 30);
}