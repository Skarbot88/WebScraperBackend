using API.Extensions;
using Core.DTOs;
using Core.Interfaces;
using Core.Models;

namespace API.Services;

/// <summary>
/// Main search service - orchestrates the search process (Single Responsibility)
/// </summary>
public class SearchService : ISearchService
{
    private readonly ISearchEngineService _searchEngine;
    private readonly ISearchResultRepository _repository;
    private readonly IPositionAnalyser _positionAnalyser;
    private readonly ILogger<SearchService> _logger;

    public SearchService(
        ISearchEngineService searchEngine,
        ISearchResultRepository repository,
        IPositionAnalyser positionAnalyser,
        ILogger<SearchService> logger)
    {
        _searchEngine = searchEngine;
        _repository = repository;
        _positionAnalyser = positionAnalyser;
        _logger = logger;
    }

    public async Task<SearchResultDto> SearchAsync(string searchTerm, string targetUrl)
    {
        try
        {
            _logger.LogInformation($"Starting search for term: {searchTerm}");

            // Get search results from search engine
            var searchResults = await _searchEngine.SearchAsync(searchTerm);

            // Analyze positions
            var positions = _positionAnalyser.FindPositions(searchResults, targetUrl);

            // Create domain model
            var searchResult = new SearchResult
            {
                Positions = positions.ToString(),
                SearchTerm = searchTerm.Trim(),
                TargetUrl = targetUrl.Trim().ToLowerInvariant(),
                SearchDate = DateTime.UtcNow,
                TotalResults = searchResults.Count()
            };

            searchResult.SetPositionsArray(positions);

            // Save to repository
            await _repository.SaveSearchResultAsync(searchResult);

            // Return DTO
            return searchResult.ToDto(); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error performing search for term: {searchTerm}");
            throw;
        }
    }
    public async Task<IEnumerable<SearchResultDto>> GetSearchHistoryAsync(string? searchTerm = null, int days = 30)
    {
        try
        {
            var history = await _repository.GetSearchHistoryAsync(searchTerm, days);

            return history.ToDtos();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving search history");
            throw;
        }
    }

    public async Task<IEnumerable<TrendDataDto>> GetTrendsAsync(string searchTerm, int days = 30)
    {
        try
        {
            var searchResults = await _repository.GetSearchResultsByTermAsync(searchTerm, days);
                
            return searchResults
                .GroupBy(sr => sr.SearchDate.Date)
                .Select(group => new TrendDataDto
                {
                    Date = group.Key,
                    BestPosition = group.SelectMany(g => g.GetPositionsArray()).DefaultIfEmpty(0).Min(),
                    TotalOccurrences = group.SelectMany(g => g.GetPositionsArray()).Count(),
                    Positions = group.SelectMany(g => g.GetPositionsArray()).OrderBy(p => p)
                })
                .OrderBy(t => t.Date);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving trends for term: {searchTerm}");
            throw;
        }
    }
}