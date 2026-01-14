using PlayMarketScraper;
namespace PlayMarketScraper;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: PlayStoreScraper <keyword> <country>");
            Console.WriteLine("Example: PlayStoreScraper poker US");
            return;
        }

        var keyword = args[0];
        var country = args[1];

        Console.WriteLine($"Search apps for a keyword: {keyword}");
        Console.WriteLine($"Country: {country}");
        Console.WriteLine();

        var cookies = Environment.GetEnvironmentVariable("PLAY_MARKET_COOKIES");

        using var scraper = new PlayStoreScraper(cookies);

        try
        {
            var packageNames = await scraper.SearchAppsAsync(keyword, country);

            Console.WriteLine($"Total number of apps found: {packageNames.Count}");
            Console.WriteLine("====================");
            foreach (var packageName in packageNames)
            {
                Console.WriteLine(packageName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }
}