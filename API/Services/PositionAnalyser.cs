using System.Web;
using Core.Interfaces;

namespace API.Services;

/// <summary>
/// Position analyser service - finds positions of target URL in results
/// </summary>
public class PositionAnalyser : IPositionAnalyser
{
    private readonly ILogger<PositionAnalyser> _logger;

    public PositionAnalyser(ILogger<PositionAnalyser> logger)
    {
        _logger = logger;
    }

    public IEnumerable<int> FindPositions(IEnumerable<string> searchResults, string targetUrl)
    {
        var positions = new List<int>();
        var cleanTargetUrl = CleanUrlForComparison(targetUrl);
        
        _logger.LogInformation($"Looking for target URL: {cleanTargetUrl}");

        var resultsList = searchResults.ToList();
        
        for (int i = 0; i < resultsList.Count; i++)
        {
            if (IsUrlMatch(resultsList[i], cleanTargetUrl))
            {
                positions.Add(i + 1); 
                _logger.LogInformation($"Found match at position {i + 1}: {resultsList[i]}");
            }
        }

        return positions;
    }

    public bool IsUrlMatch(string searchResultUrl, string targetUrl)
    {
        var cleanSearchUrl = CleanUrlForComparison(ExtractDomain(searchResultUrl));
        var cleanTargetUrl = CleanUrlForComparison(targetUrl);

        return cleanSearchUrl.Equals(cleanTargetUrl, StringComparison.OrdinalIgnoreCase);
    }
    
    private static readonly HashSet<string> KnownMultiPartTlds = new()
    {
        "co.uk", "gov.uk", "ac.uk", "org.uk",
        "com.au", "net.au", "org.au"
    };

    private static string ExtractDomain(string googleUrl)
    {
        if (string.IsNullOrWhiteSpace(googleUrl))
            return string.Empty;

        var query = googleUrl[(googleUrl.IndexOf('?') + 1)..];
        var target = HttpUtility.ParseQueryString(query)["q"];
        if (string.IsNullOrEmpty(target))
            return string.Empty;

        var host = new Uri(target).Host.ToLowerInvariant();

        var parts = host.Split('.');
        if (parts.Length < 2)
            return host;

        var lastTwo = $"{parts[^2]}.{parts[^1]}";
        if (KnownMultiPartTlds.Contains(lastTwo) && parts.Length >= 3)
        {
            return $"{parts[^3]}.{lastTwo}";
        }

        return lastTwo;
    }

    private string CleanUrlForComparison(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        return url.Replace("http://", "")
                 .Replace("https://", "")
                 .Replace("www.", "")
                 .Split('/')[0]
                 .Split('?')[0]
                 .Split('#')[0]
                 .ToLowerInvariant()
                 .Trim();
    }
}