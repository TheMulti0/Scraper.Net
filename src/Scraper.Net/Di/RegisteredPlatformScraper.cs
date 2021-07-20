namespace Scraper.Net
{
    internal record RegisteredPlatformScraper(string Platform, IPlatformScraper Scraper);
}