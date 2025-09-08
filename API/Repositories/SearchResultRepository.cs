using API.Data;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

/// <summary>
/// Repository implementation for SearchResult entities
/// Follows Repository pattern and Dependency Inversion principle
/// </summary>
public class SearchResultRepository : ISearchResultRepository
{
    private readonly WebScraperContext _context;
    private readonly ILogger<SearchResultRepository> _logger;

    public SearchResultRepository(WebScraperContext context, ILogger<SearchResultRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SearchResult> SaveSearchResultAsync(SearchResult searchResult)
    {
        try
        {
            _logger.LogInformation($"Saving search result for term: {searchResult.SearchTerm}");

            _context.SearchResults.Add(searchResult);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Search result saved with ID: {searchResult.Id}");
            
            return searchResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving search result");
            throw;
        }
    }

    public async Task<IEnumerable<SearchResult>> GetSearchHistoryAsync(string? searchTerm = null, int days = 30)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            
            var query = _context.SearchResults
                .Where(sr => sr.SearchDate >= cutoffDate);
    
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(sr => sr.SearchTerm.Contains(searchTerm));
            }
    
            var results = await query
                .OrderByDescending(sr => sr.SearchDate)
                .Take(100) 
                .ToListAsync();
    
            _logger.LogInformation($"Retrieved {results.Count} search history records");
            
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving search history");
            throw;
        }
    }

    public async Task<IEnumerable<SearchResult>> GetSearchResultsByTermAsync(string searchTerm, int days = 30)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            
            var results = await _context.SearchResults
                .Where(sr => sr.SearchTerm == searchTerm && sr.SearchDate >= cutoffDate)
                .OrderByDescending(sr => sr.SearchDate)
                .ToListAsync();

            _logger.LogInformation($"Retrieved {results.Count} search results for term: {searchTerm}");
            
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving search results for term: {searchTerm}");
            throw;
        }
    }
}