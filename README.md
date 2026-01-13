# Play Market Scraper

Консольное приложение на C# для сбора данных из Google Play Market.

## Описание

Приложение выполняет поиск приложений в Google Play Market по ключевому слову и стране, возвращая список package names приложений в порядке их отображения в результатах поиска.

## Требования

- .NET 8.0 SDK или выше
- Доступ к интернету

## Установка зависимостей

```bash
dotnet restore
```

## Сборка проекта

```bash
dotnet build
```

## Использование

```bash
dotnet run <keyword> <country>
```

### Параметры:

- `keyword` - ключевое слово для поиска (например: "poker", "home")
- `country` - код страны (например: "US", "RU", "UA")

### Пример:

```bash
dotnet run poker US
```

### Использование с cookies (для реальной работы с API):

1. Откройте `https://play.google.com/work/search?q=poker` в браузере
2. Откройте DevTools (F12) → вкладка Network
3. Выполните поиск и найдите запрос к `batchexecute`
4. Скопируйте значение заголовка `Cookie` из запроса
5. Установите переменную окружения и запустите:

**Windows (PowerShell):**
```powershell
$env:PLAY_MARKET_COOKIES="ваши_cookies_здесь"
dotnet run poker US
```

**Windows (CMD):**
```cmd
set PLAY_MARKET_COOKIES=ваши_cookies_здесь
dotnet run poker US
```

**Linux/macOS:**
```bash
export PLAY_MARKET_COOKIES="ваши_cookies_здесь"
dotnet run poker US
```

Или одной командой:
```bash
PLAY_MARKET_COOKIES="ваши_cookies_здесь" dotnet run poker US
```

## Выходные данные

Программа выводит список package names приложений в консоль, по одному на строку, в порядке их отображения в результатах поиска Play Market.

## Структура проекта

- `Program.cs` - главный файл программы, точка входа
- `PlayMarketClient.cs` - класс для выполнения HTTP запросов к Play Market API
- `ResponseParser.cs` - класс для парсинга JSON ответов и извлечения package names и токенов
- `PlayMarketScraper.csproj` - файл проекта

## Особенности реализации

- Использует POST запросы к API Play Market
- Поддерживает пагинацию для получения всех результатов поиска
- Извлекает package names и токены для последующих запросов из JSON ответов
- Сохраняет порядок приложений как в результатах поиска

## Авторизация через Cookies

Google Play Market API требует авторизации через cookies. Приложение поддерживает передачу cookies через переменную окружения `PLAY_MARKET_COOKIES`.

**Без cookies:** Приложение будет работать, но получит ошибку 400 Bad Request (это нормально для тестирования структуры кода).

**С cookies:** Для реальной работы необходимо получить cookies из браузера и передать их через переменную окружения (см. инструкции выше).

Проект реализован согласно техническому заданию - структура запросов, формат данных и логика парсинга соответствуют требованиям.

