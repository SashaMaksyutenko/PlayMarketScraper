using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace PlayMarketScraper;

public class PlayMarketClient
{
    private readonly HttpClient _httpClient;
    private readonly CookieContainer _cookieContainer;
    private const string BaseUrl = "https://play.google.com/_/PlayEnterpriseWebStoreUi/data/batchexecute";
    private const string BatchId = "boq_playenterprisewebuiserver_20251214.04_p0";
    private const string FirstRequestMethod = "lGYRle";
    private const string SubsequentRequestMethod = "qnKh0b";

    public PlayMarketClient(string? cookies = null)
    {
        _cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler
        {
            CookieContainer = _cookieContainer,
            UseCookies = true
        };

        _httpClient = new HttpClient(handler);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
        _httpClient.DefaultRequestHeaders.Add("Origin", "https://play.google.com");
        _httpClient.DefaultRequestHeaders.Add("Referer", "https://play.google.com/");

        if (!string.IsNullOrEmpty(cookies))
        {
            SetCookies(cookies);
        }
    }

    public void SetCookies(string cookieString)
    {
        if (string.IsNullOrWhiteSpace(cookieString))
            return;

        var uri = new Uri("https://play.google.com");
        
        // Разделяем cookies по точке с запятой
        var cookies = cookieString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var cookie in cookies)
        {
            var trimmed = cookie.Trim();
            if (string.IsNullOrEmpty(trimmed))
                continue;

            // Разделяем на имя и значение (берем первое = как разделитель)
            var equalIndex = trimmed.IndexOf('=');
            if (equalIndex <= 0 || equalIndex >= trimmed.Length - 1)
                continue;

            var name = trimmed.Substring(0, equalIndex).Trim();
            var value = trimmed.Substring(equalIndex + 1).Trim();
            
            if (string.IsNullOrEmpty(name))
                continue;

            try
            {
                // Создаем cookie с правильным доменом и путем
                var cookieObj = new Cookie(name, value)
                {
                    Domain = ".google.com",
                    Path = "/"
                };
                
                _cookieContainer.Add(uri, cookieObj);
            }
            catch (CookieException)
            {
                // Игнорируем некорректные cookies
            }
        }
    }

    public async Task<string> SearchAppsAsync(string keyword, string country, string? continuationToken = null)
    {
        var countryCode = GetCountryCode(country);
        var languageCode = GetLanguageCode(country);
        
        var queryParams = new Dictionary<string, string>
        {
            { "source-path", "/work/search" },
            { "bl", BatchId },
            { "hl", languageCode },
            { "gl", countryCode },
            { "authuser", "0" },
            { "rt", "c" },
            { "rpcids", continuationToken == null ? FirstRequestMethod : SubsequentRequestMethod }
        };

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var url = $"{BaseUrl}?{queryString}";

        var requestBody = continuationToken == null 
            ? BuildFirstRequest(keyword)
            : BuildSubsequentRequest(continuationToken);

        var content = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = await _httpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Ошибка HTTP {response.StatusCode}: {errorContent.Substring(0, Math.Min(500, errorContent.Length))}");
            throw new HttpRequestException($"Request failed with status {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync();
    }

    private string BuildFirstRequest(string keyword)
    {
        var requestData = $"[null,null,null,null,[null,1]],[[10,[10,50]],null,null,[96,108,72,100,27,177,183,222,8,57,169,110,11,184,16,1,139,152,194,165,68,163,211,9,71,31,195,12,64,151,150,148,113,104,55,56,145,32,34,10,122]],[\"{keyword}\"],4,null,null,null,[null,1]]";
        var jsonRequest = $"[[[\"{FirstRequestMethod}\",\"[[{requestData}]]\",null,\"1\"]]]";
        return $"f.req={Uri.EscapeDataString(jsonRequest)}";
    }

    private string BuildSubsequentRequest(string continuationToken)
    {
        var jsonRequest = $"[[[\"{SubsequentRequestMethod}\",\"[[null,[null,[null,[null,\"{continuationToken}\"]]],null,null,[null,1]]]\",null,\"1\"]]]";
        return $"f.req={Uri.EscapeDataString(jsonRequest)}";
    }

    private string GetCountryCode(string country)
    {
        return country.ToUpper();
    }

    private string GetLanguageCode(string country)
    {
        var languageMap = new Dictionary<string, string>
        {
            { "RU", "ru" },
            { "US", "en-US" },
            { "UA", "uk" }
        };

        return languageMap.GetValueOrDefault(country.ToUpper(), "en-US");
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

