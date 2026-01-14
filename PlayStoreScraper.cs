using System.Net;
using System.Text.RegularExpressions;

namespace PlayMarketScraper;

public class PlayStoreScraper : IDisposable
{
    private readonly HttpClient _httpClient;

    public PlayStoreScraper(string? cookies = null)
    {
        var handler = new HttpClientHandler
        {
            UseCookies = false,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        _httpClient = new HttpClient(handler);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        _httpClient.DefaultRequestHeaders.Add("Accept", "text/html");
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

        if (!string.IsNullOrEmpty(cookies))
            _httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
    }

    public async Task<List<string>> SearchAppsAsync(string keyword, string country)
    {
        var url = $"https://play.google.com/store/search?q={Uri.EscapeDataString(keyword)}&c=apps&gl={country}&hl=en";
        var html = await _httpClient.GetStringAsync(url);

        var matches = Regex.Matches(html, @"\/store\/apps\/details\?id=([a-zA-Z0-9\._]+)");
        return matches.Select(m => m.Groups[1].Value).Distinct().ToList();
    }

    public void Dispose() => _httpClient?.Dispose();
}