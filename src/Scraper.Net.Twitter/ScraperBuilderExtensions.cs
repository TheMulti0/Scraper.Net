using System;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.Twitter
{
    public static class ScraperBuilderExtensions
    {
        public static ScraperBuilder AddTwitter(
            this ScraperBuilder builder,
            TwitterConfig config = null,
            string platform = "twitter")
        {
            return builder
                .AddScraper(provider =>
                {
                    config ??= provider.GetService<TwitterConfig>() ?? throw new ArgumentNullException(nameof(config));
                    var scraper = new TwitterScraper(config);

                    return (scraper, platform);
                });
        }
    }
}