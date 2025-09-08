using Core.DTOs;

namespace Core.Interfaces;

/// <summary>
/// Main search service interface - Single Responsibility Principle
/// </summary>
public interface ISearchService
{
    Task<SearchResultDto> SearchAsync(string searchTerm, string targetUrl);
    Task<IEnumerable<SearchResultDto>> GetSearchHistoryAsync(string? searchTerm = null, int days = 30);
    Task<IEnumerable<TrendDataDto>> GetTrendsAsync(string searchTerm, int days = 30);
    
}