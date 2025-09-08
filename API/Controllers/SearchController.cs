using API.DTOs;
using Core.DTOs;
using Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly IValidator<SearchRequestDto> _validator;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        ISearchService searchService,
        IValidator<SearchRequestDto> validator,
        ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Performs  search that returns position of url in search results list
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SearchResultDto>> Search([FromBody] SearchRequestDto request)
    {
        try
        {
            //Validate request
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse<SearchResultDto>.ErrorResponse("Validation failed", errors));
            }

            _logger.LogInformation($"Performing search for term: {request.SearchTerm}, URL: {request.TargetUrl}");

            var result = await _searchService.SearchAsync(request.SearchTerm, request.TargetUrl);
                
            _logger.LogInformation($"Search completed. Found positions: {string.Join(", ", result.Positions)}");
                
            return Ok(ApiResponse<SearchResultDto>.SuccessResponse(result, "Search completed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search");
            return StatusCode(500, ApiResponse<SearchResultDto>.ErrorResponse("An error occurred while performing the search", ex.Message));
        }
    }
    
    /// <summary>
    /// Gets search history with optional filtering
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SearchResultDto>>>> GetHistory(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int days = 30)
    {
        try
        {
            if (days < 1 || days > 365)
            {
                return BadRequest(ApiResponse<IEnumerable<SearchResultDto>>.ErrorResponse("Days must be between 1 and 365"));
            }

            _logger.LogInformation($"Getting search history for term: {searchTerm}, days: {days}");
            
            var history = await _searchService.GetSearchHistoryAsync(searchTerm, days);
            
            return Ok(ApiResponse<IEnumerable<SearchResultDto>>.SuccessResponse(history, "History retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving search history");
            return StatusCode(500, ApiResponse<IEnumerable<SearchResultDto>>.ErrorResponse("An error occurred while retrieving search history", ex.Message));
        }
    }

    /// <summary>
    /// Gets trend data for a specific search term
    /// </summary>
    [HttpGet("trends/{searchTerm}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TrendDataDto>>>> GetTrends(
        string searchTerm,
        [FromQuery] int days = 30)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(ApiResponse<IEnumerable<TrendDataDto>>.ErrorResponse("Search term is required"));
            }

            if (days < 1 || days > 365)
            {
                return BadRequest(ApiResponse<IEnumerable<TrendDataDto>>.ErrorResponse("Days must be between 1 and 365"));
            }

            _logger.LogInformation($"Getting trends for term: {searchTerm}, days: {days}");
            
            var trends = await _searchService.GetTrendsAsync(searchTerm, days);
            
            return Ok(ApiResponse<IEnumerable<TrendDataDto>>.SuccessResponse(trends, "Trends retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving trends");
            return StatusCode(500, ApiResponse<IEnumerable<TrendDataDto>>.ErrorResponse("An error occurred while retrieving trends", ex.Message));
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { 
            status = "healthy", 
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            service = "SEO Position Tracker API"
        });
    }
}