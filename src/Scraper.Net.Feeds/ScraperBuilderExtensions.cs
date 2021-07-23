namespace Scraper.Net.Feeds
{
    public static class ScraperBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="FeedsScraper"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="platform"></param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static ScraperBuilder AddFeeds(
            this ScraperBuilder builder,
            string platform = "feeds")
        {
            return builder
                .AddScraper(_ =>
                {
                    var scraper = new FeedsScraper();

                    return (scraper, platform);
                });
        }
    }
}