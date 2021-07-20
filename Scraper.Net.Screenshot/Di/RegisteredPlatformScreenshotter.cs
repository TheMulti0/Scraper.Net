namespace Scraper.Net.Screenshot
{
    internal record RegisteredPlatformScreenshotter(string Platform, IPlatformScreenshotter Screenshotter);
}