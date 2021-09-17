using System;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.Youtube
{
    public static class ScraperBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="YoutubeScraper"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <param name="platform"></param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static ScraperBuilder AddYoutube(
            this ScraperBuilder builder,
            YoutubeConfig config = null,
            string platform = "youtube")
        {
            return builder
                .AddScraper(provider =>
                {
                    config ??= provider.GetService<YoutubeConfig>() ?? throw new ArgumentNullException(nameof(config));
                    
                    var scraper = ActivatorUtilities.CreateInstance<YoutubeScraper>(provider, config);

                    return (scraper, platform);
                });
        }
    }
}