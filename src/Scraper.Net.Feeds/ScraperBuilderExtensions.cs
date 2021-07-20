namespace Scraper.Net.Feeds
{
    public static class ScraperBuilderExtensions
    {
        public static ScraperBuilder AddFeed(
            this ScraperBuilder builder,
            string platform = "feeds")
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