
using PlayMarketScraper;

namespace PlayMarketScraper;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: dotnet run <keyword> <country>");
            Console.WriteLine("Example: dotnet run poker US");
            return;
        }

        var keyword = args[0];
        var country = args[1];

        Console.WriteLine($"Search apps for a keyword: {keyword}");
        Console.WriteLine($"Country: {country}");
        Console.WriteLine();

        // HTML scraper (works without cookies)
        using var scraper = new PlayStoreScraper();
        var apps = await scraper.SearchAppsAsync(keyword, country);

        Console.WriteLine($"Found apps (HTML scraper): {apps.Count}");
        Console.WriteLine("======================");
        foreach (var app in apps)
            Console.WriteLine(app);

        // Batch client (requires cookies from DevTools)
        // Uncomment if you want to test with cookies
        /*
        using var batchClient = new PlayMarketClient("<your cookies here>", "teXCtc");
        var batchApps = await batchClient.SearchAsync(keyword, country);

        Console.WriteLine($"Found apps (Batch API): {batchApps.Count}");
        Console.WriteLine("======================");
        foreach (var app in batchApps)
            Console.WriteLine(app);
        */
        // Batch client (requires cookies from DevTools)
        using var batchClient = new PlayMarketClient(
            "HSID=ARi0bmx52gSnf-631; SSID=AkyRbIfjXRwiQiLtR; APISID=1-lqtMBbxFAwjLYb/ABeQgJMLorOVG3v6J; SAPISID=h9-pQZzj7wxbvjIq/AXq5urfzWmSRrOLoz; __Secure-1PAPISID=h9-pQZzj7wxbvjIq/AXq5urfzWmSRrOLoz; __Secure-3PAPISID=h9-pQZzj7wxbvjIq/AXq5urfzWmSRrOLoz; SEARCH_SAMESITE=CgQIwZ8B; __Secure-BUCKET=CJED; SID=g.a0005AgDlxP7o-neg9k3WnVX9cHwuUeZZqsHp7jwm2Ll-W-QWqEKVwj7PGiIxPYVSX_oxmRU6wACgYKAeoSARYSFQHGX2MimixkW9ubd93TwjBwccXGcBoVAUF8yKrmD5bYXGuTePhq4E5Ejlpy0076; __Secure-1PSID=g.a0005AgDlxP7o-neg9k3WnVX9cHwuUeZZqsHp7jwm2Ll-W-QWqEKEu70Fh3mXhxPimvy5wxd9AACgYKAX4SARYSFQHGX2Mi0y6YH4Zh05IPxXmY0V_cehoVAUF8yKoa_mmljvGBr8LSAoFFDOds0076; __Secure-3PSID=g.a0005AgDlxP7o-neg9k3WnVX9cHwuUeZZqsHp7jwm2Ll-W-QWqEKmI4x3uqMVOr0sixDUBG7XgACgYKAf4SARYSFQHGX2Mi6BELqmzeFvVxSTICyrvDWxoVAUF8yKoyZcCcRzWcWRqMq0_jfnoB0076; __Secure-ENID=30.SE=I3dDgmO0fHSiQyob_zNmTEz-bTK4l4vSorlzVTilW3xuRxNvwOPp4QJZd35x8GlC7zxif0h00ErrGQgTm88BxZLpA1CaHjJX5WUJAgRQaGTw3ORvAYjSq0LKFmJY--_yJJl12PVFqr_KGPW5l7ynjHEzUIAOjE9WVlw0EgfZphSxIqU7fPkxAb9YrPrRmBhkDWycwr0oL-gXIMDau8AAAicxd9mPx_pw8q8f_YAoXd08lPCqMb06BE9-hVH-rBjisHBtP8xn1hPuJygkzHCq2QE_3co_jwpvPruF73VOOQhJBongzT8t28LkgDq8Tx_o9lT1zSSIVrstMiaACPo; ...",
            "teXCtc" // актуальний rpcId із DevTools
        );

        var batchApps = await batchClient.SearchAsync(keyword, country);

        Console.WriteLine($"Found apps (Batch API): {batchApps.Count}");
        Console.WriteLine("======================");
        foreach (var app in batchApps)
            Console.WriteLine(app);
    }
}