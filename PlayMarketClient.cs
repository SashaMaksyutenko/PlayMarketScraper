
using System.Net;
using System.Text;
using System.Text.Json;

namespace PlayMarketScraper;

public class PlayMarketClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly string _rpcId;

    // Base URL for batch API (can be PlayStoreUi or PlayEnterpriseWebStoreUi)
    private const string BatchUrl = "https://play.google.com/_/PlayStoreUi/data/batchexecute";

    public PlayMarketClient(string? cookies, string rpcId)
    {
        _rpcId = rpcId;

        var handler = new HttpClientHandler
        {
            UseCookies = false,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        _http = new HttpClient(handler);
        _http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        _http.DefaultRequestHeaders.Add("Accept-Language", "en-US");

        if (!string.IsNullOrEmpty(cookies))
            _http.DefaultRequestHeaders.Add("Cookie", cookies);
    }

    public async Task<List<string>> SearchAsync(string keyword, string country)
    {
        var result = new List<string>();

        var response = await SendInitialRequest(keyword);
        ParseResponse(response, result, out var token);

        while (!string.IsNullOrEmpty(token))
        {
            response = await SendNextPageRequest(token);
            ParseResponse(response, result, out token);
        }

        return result;
    }

    private async Task<string> SendInitialRequest(string keyword)
    {
        var body = $"f.req=[[[" +
                   $"\"{_rpcId}\"," +
                   $"\"[[\\\"{keyword}\\\",null,[2,3],null,null,1]]\"," +
                   $"null,\"generic\"]]]&rpcids={_rpcId}";

        return await PostAsync(body);
    }

    private async Task<string> SendNextPageRequest(string token)
    {
        var body = $"f.req=[[[" +
                   $"\"{_rpcId}\"," +
                   $"\"[{token}]\"," +
                   $"null,\"generic\"]]]&rpcids={_rpcId}";

        return await PostAsync(body);
    }

    private async Task<string> PostAsync(string body)
    {
        var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = await _http.PostAsync(BatchUrl, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private void ParseResponse(string raw, List<string> apps, out string? nextToken)
    {
        nextToken = null;
        var jsonStart = raw.IndexOf('[');
        if (jsonStart == -1) return;

        var json = raw.Substring(jsonStart);
        using var doc = JsonDocument.Parse(json);

        foreach (var element in doc.RootElement.EnumerateArray())
        {
            var text = element.ToString();

            if (text.Contains("details?id="))
            {
                var idStart = text.IndexOf("details?id=") + 11;
                var idEnd = text.IndexOf("\"", idStart);
                if (idEnd > idStart)
                {
                    var id = text.Substring(idStart, idEnd - idStart);
                    if (!apps.Contains(id))
                        apps.Add(id);
                }
            }

            if (text.Contains("nextPageToken"))
                nextToken = text;
        }
    }

    public void Dispose() => _http.Dispose();
}