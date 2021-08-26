using System;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.Twitter
{
    public static class ScraperBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="TwitterScraper"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <param name="platform"></param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static ScraperBuilder AddTwitter(
            this ScraperBuilder builder,
            TwitterConfig config = null,
            string platform = "twitter")
        {
            return builder
                .AddScraper(provider =>
                {
                    config ??= provider.GetService<TwitterConfig>() ?? throw new ArgumentNullException(nameof(config));
                    
                    var scraper = ActivatorUtilities.CreateInstance<TwitterScraper>(provider, config);

                    return (scraper, platform);
                });
        }
    }
}