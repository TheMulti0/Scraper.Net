namespace Scraper.Net.Feed
{
    public static class ScraperBuilderExtensions
    {
        public static ScraperBuilder AddFeed(
            this ScraperBuilder builder,
            string platform = "feed")
        {
            return builder
                .AddScraper(_ =>
                {
                    var scraper = new FeedScraper();

                    return (scraper, platform);
                });
        }
    }
}