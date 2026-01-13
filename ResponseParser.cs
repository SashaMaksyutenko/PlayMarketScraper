using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace PlayMarketScraper;

public class ResponseParser
{
    private static readonly Regex PackageNamePattern = new Regex(@"^[a-z][a-z0-9_]*(\.[a-z][a-z0-9_]*)+$", RegexOptions.Compiled);

    public (List<string> PackageNames, string? ContinuationToken) ParseResponse(string response)
    {
        var packageNames = new List<string>();
        string? continuationToken = null;

        try
        {
            // Ответ приходит в формате: ["wrb.fr","lGYRle","[JSON_STRING]",...]
            var jsonArray = JArray.Parse(response);
            
            if (jsonArray.Count > 2 && jsonArray[2] != null)
            {
                var jsonString = jsonArray[2].ToString();
                var dataArray = JArray.Parse(jsonString);
                
                // Извлекаем package names и токен
                var foundPackages = new HashSet<string>();
                ExtractPackageNames(dataArray, foundPackages);
                packageNames.AddRange(foundPackages);
                
                continuationToken = ExtractContinuationToken(dataArray);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при парсинге ответа: {ex.Message}");
        }

        return (packageNames, continuationToken);
    }

    private void ExtractPackageNames(JToken token, HashSet<string> packageNames)
    {
        if (token == null) return;

        try
        {
            if (token is JValue value && value.Type == JTokenType.String)
            {
                var str = value.ToString();
                if (IsPackageName(str))
                {
                    packageNames.Add(str);
                }
            }
            else if (token is JArray array)
            {
                foreach (var item in array)
                {
                    ExtractPackageNames(item, packageNames);
                }
            }
            else if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    ExtractPackageNames(property.Value, packageNames);
                }
            }
        }
        catch
        {
            // Игнорируем ошибки при рекурсивном обходе
        }
    }

    private bool IsPackageName(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return false;
        if (str.Length < 3 || str.Length > 200) return false;
        if (str.StartsWith("http")) return false;
        if (str.Contains(" ")) return false;
        if (str.Contains("/")) return false;
        
        // Package name должен содержать хотя бы одну точку и соответствовать паттерну
        return str.Contains(".") && PackageNamePattern.IsMatch(str);
    }

    private string? ExtractContinuationToken(JToken token)
    {
        if (token == null) return null;

        try
        {
            if (token is JValue value && value.Type == JTokenType.String)
            {
                var str = value.ToString();
                // Токен обычно очень длинный (более 100 символов) и содержит base64-подобные символы
                if (str.Length > 100 && (str.Contains("=") || str.Contains("-") || str.Contains("_")))
                {
                    // Проверяем, что это не package name
                    if (!IsPackageName(str))
                    {
                        return str;
                    }
                }
            }
            else if (token is JArray array)
            {
                // Ищем токен в массивах, проверяя строковые значения
                foreach (var item in array)
                {
                    var tokenFromNested = ExtractContinuationToken(item);
                    if (tokenFromNested != null)
                    {
                        return tokenFromNested;
                    }
                }
            }
            else if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    var tokenFromNested = ExtractContinuationToken(property.Value);
                    if (tokenFromNested != null)
                    {
                        return tokenFromNested;
                    }
                }
            }
        }
        catch
        {
            // Игнорируем ошибки
        }

        return null;
    }
}
