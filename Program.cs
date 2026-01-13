using PlayMarketScraper;

namespace PlayMarketScraper;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Использование: PlayMarketScraper <keyword> <country>");
            Console.WriteLine("Пример: PlayMarketScraper poker US");
            return;
        }

        var keyword = args[0];
        var country = args[1];

        Console.WriteLine($"Поиск приложений для ключевого слова: {keyword}");
        Console.WriteLine($"Страна: {country}");
        Console.WriteLine();

        // Получаем cookies из переменной окружения (опционально)
        var cookies = Environment.GetEnvironmentVariable("PLAY_MARKET_COOKIES");
        var client = new PlayMarketClient(cookies);
        var parser = new ResponseParser();
        var allPackageNames = new List<string>();
        string? continuationToken = null;
        int pageNumber = 1;

        try
        {
            do
            {
                Console.WriteLine($"Получение страницы {pageNumber}...");
                
                var response = await client.SearchAppsAsync(keyword, country, continuationToken);
                var (packageNames, nextToken) = parser.ParseResponse(response);
                
                Console.WriteLine($"Найдено приложений на странице: {packageNames.Count}");
                
                allPackageNames.AddRange(packageNames);
                continuationToken = nextToken;
                pageNumber++;

                // Ограничение для предотвращения бесконечного цикла
                if (pageNumber > 100)
                {
                    Console.WriteLine("Достигнут лимит страниц (100)");
                    break;
                }

                // Небольшая задержка между запросами
                await Task.Delay(500);
                
            } while (!string.IsNullOrEmpty(continuationToken));

            Console.WriteLine();
            Console.WriteLine($"Всего найдено приложений: {allPackageNames.Count}");
            Console.WriteLine();
            Console.WriteLine("Список package names:");
            Console.WriteLine("====================");
            
            foreach (var packageName in allPackageNames)
            {
                Console.WriteLine(packageName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
        finally
        {
            client.Dispose();
        }
    }
}

