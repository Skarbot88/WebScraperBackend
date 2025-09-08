using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Core.Interfaces;
using HtmlAgilityPack;

namespace API.Services
{
    /// <summary>
    /// Google scraping service - handles web scraping logic (Single Responsibility)
    /// </summary>
    public class GoogleScrapingService : ISearchEngineService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleScrapingService> _logger;
        private static Random _random = new Random();

        public GoogleScrapingService(HttpClient httpClient, ILogger<GoogleScrapingService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            var baseAddress = new Uri("https://www.google.com/");

            _httpClient.BaseAddress = baseAddress;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            _httpClient.DefaultRequestHeaders.Add("Cookie", "CONSENT=PENDING+987; SOCS=CAESHAgBEhIaAB");
        }

        private static string GetRandomUserAgent()
        {
            var lynxVersion = $"Lynx/{2 + _random.Next(0, 2)}.{8 + _random.Next(0, 2)}.{_random.Next(0, 3)}";
            var libwwwVersion = $"libwww-FM/{2 + _random.Next(0, 2)}.{13 + _random.Next(0, 3)}";
            var sslMmVersion = $"SSL-MM/{1 + _random.Next(0, 1)}.{3 + _random.Next(0, 3)}";
            var opensslVersion = $"OpenSSL/{1 + _random.Next(0, 3)}.{_random.Next(0, 5)}.{_random.Next(0, 10)}";

            return $"{lynxVersion} {libwwwVersion} {sslMmVersion} {opensslVersion}";
        }

        public async Task<IEnumerable<string>> SearchAsync(string searchTerm, int maxResults = 100)
        {
            try
            {
                var url = $"search?num={maxResults}&q={Uri.EscapeDataString(searchTerm)}";
                var response = await _httpClient.GetStringAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(response);
                var results = await ScrapeUrlsAsync(doc);
                
                if (results != null)
                {
                    var count = 1;
                    foreach (var result in results)
                    {
                        Console.WriteLine($"{result}");
                        count++;
                    }
                }
                else
                {
                    throw new NullReferenceException("No results found or selector failed.");
                }
                
                return results;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while scraping Google");
                throw new Exception("Failed to retrieve search results from Google", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while scraping Google search results");
                throw;
            }
        }
        
        private static async Task<List<string>> ScrapeUrlsAsync(HtmlDocument doc)
        {

            var links = new List<string>();
            
            foreach (var anchor in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                string hrefValue = anchor.GetAttributeValue("href", string.Empty);

                if (!string.IsNullOrEmpty(hrefValue) && hrefValue.StartsWith("/url?q=https://"))
                {
                    links.Add(hrefValue);
                }
            }

            return links;
        }
        
        private static Uri GetUrl(string query) => new Uri($"https://www.google.com/search?q={query}");

        private IEnumerable<string> ExtractSearchResultUrls(string html)
        {
            var urls = new List<string>();

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var linkSelectors = new[]
                {
                    "//div[@class='g']//a/@href",
                    "//div[@class='yuRUbf']//a/@href",
                    "//div[@class='egMi0 kCrYT']//a/@href",
                    "//h3/parent::a/@href"
                };

                foreach (var selector in linkSelectors)
                {
                    var links = doc.DocumentNode.SelectNodes(selector);
                    if (links != null)
                    {
                        foreach (var link in links)
                        {
                            var href = link.GetAttributeValue("href", "");
                            if (!string.IsNullOrEmpty(href) && IsValidSearchResultUrl(href))
                            {
                                var cleanUrl = CleanUrl(href);
                                if (!string.IsNullOrEmpty(cleanUrl) && !urls.Contains(cleanUrl))
                                {
                                    urls.Add(cleanUrl);
                                }
                            }
                        }
                    }
                }

                _logger.LogInformation($"Extracted {urls.Count} URLs from search results");
                return urls.Take(100); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing HTML content");
                return new List<string>();
            }
        }

        private bool IsValidSearchResultUrl(string url)
        {
            return !string.IsNullOrEmpty(url) &&
                   !url.StartsWith("/search") &&
                   !url.StartsWith("#") &&
                   !url.Contains("google.") &&
                   !url.Contains("youtube.com") &&
                   (url.StartsWith("http") || url.StartsWith("/url?q="));
        }

        private string CleanUrl(string url)
        {
            try
            {
                if (url.StartsWith("/url?q="))
                {
                    var match = Regex.Match(url, @"[?&]q=([^&]+)");
                    if (match.Success)
                    {
                        url = Uri.UnescapeDataString(match.Groups[1].Value);
                    }
                }

                var cleanUrl = url.Replace("http://", "").Replace("https://", "").Replace("www.", "");

                cleanUrl = cleanUrl.Split('/')[0].Split('#')[0].Split('?')[0];

                return cleanUrl.ToLowerInvariant();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}

