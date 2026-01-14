Play Market Scraper

Консольний застосунок на C# для збору даних з Google Play Market.

Опис
Застосунок виконує пошук застосунків у Google Play Market за ключовим словом і країною, повертаючи список package name застосунків у тому порядку, в якому вони відображаються в результатах пошуку.

Вимоги
.NET 8.0 SDK або новіше
Доступ до інтернету

Встановлення залежностей
dotnet restore

Збірка проєкту
dotnet build

Використання
dotnet run <keyword> <country>

Параметри
keyword — ключове слово для пошуку (наприклад: "poker", "home")
country — код країни (наприклад: "US", "RU", "UA")

Приклад
dotnet run poker US

Використання з cookies (для реальної роботи з API)
1. Відкрийте https://play.google.com/work/search?q=poker у браузері
2. Відкрийте DevTools (F12) → вкладка Network
3. Виконайте пошук і знайдіть запит до batchexecute
4. Скопіюйте значення заголовка Cookie з цього запиту
5. Встановіть змінну середовища та запустіть застосунок

Windows (PowerShell)
$env:PLAY_MARKET_COOKIES="ваші_cookies_тут"
dotnet run poker US

Windows (CMD)
set PLAY_MARKET_COOKIES=ваші_cookies_тут
dotnet run poker US

Linux / macOS
export PLAY_MARKET_COOKIES="ваші_cookies_тут"
dotnet run poker US

Або однією командою
PLAY_MARKET_COOKIES="ваші_cookies_тут" dotnet run poker US

Вихідні дані
Програма виводить у консоль список package name застосунків — по одному на рядок, у тому порядку, в якому їх повертає Google Play Market.

Структура проєкту
Program.cs — головний файл програми, точка входу
PlayMarketClient.cs — клас для виконання HTTP-запитів до Play Market API
ResponseParser.cs — клас для парсингу JSON-відповідей і вилучення package name та токенів
PlayMarketScraper.csproj — файл проєкту

Особливості реалізації
Використовує POST-запити до API Play Market
Підтримує пагінацію для отримання всіх результатів пошуку
Витягує package name та токени для наступних запитів із JSON-відповідей
Зберігає порядок застосунків так, як його повертає пошук Play Market

Авторизація через Cookies
API Google Play Market потребує авторизації через cookies.
Застосунок підтримує передачу cookies через змінну середовища PLAY_MARKET_COOKIES.

Без cookies
Застосунок запуститься, але отримає помилку 400 Bad Request — це нормально для тестування структури коду.

З cookies
Для реальної роботи необхідно отримати cookies з браузера та передати їх через змінну середовища.

Проєкт реалізовано відповідно до технічного завдання — структура запитів, формат даних і логіка парсингу відповідають вимогам.
