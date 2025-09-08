using Core.DTOs;
using Core.Models;

namespace API.Extensions
{
    /// <summary>
    /// Extension methods for mapping between Core DTOs and Domain Models
    /// Keeps mapping logic organized and reusable
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// Maps SearchResult domain model to SearchResultDto
        /// </summary>
        public static SearchResultDto ToDto(this SearchResult searchResult)
        {
            return new SearchResultDto
            {
                Positions = searchResult.GetPositionsArray(),
                SearchTerm = searchResult.SearchTerm,
                TargetUrl = searchResult.TargetUrl,
                SearchDate = searchResult.SearchDate,
                TotalResults = searchResult.TotalResults
            };
        }

        /// <summary>
        /// Maps collection of SearchResult to SearchResultDto
        /// </summary>
        public static IEnumerable<SearchResultDto> ToDtos(this IEnumerable<SearchResult> searchResults)
        {
            return searchResults.Select(sr => sr.ToDto());
        }
    }
}