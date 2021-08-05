using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Scraper.Net.Facebook
{
    public static class ScraperBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="FacebookScraper"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <param name="platform"></param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static ScraperBuilder AddFacebook(
            this ScraperBuilder builder,
            FacebookConfig config = null,
            string platform = "facebook")
        {
            return builder
                .AddScraper(provider =>
                {
                    config ??= provider.GetService<FacebookConfig>() ?? new FacebookConfig();
                    var factory = provider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                    
                    var scraper = new FacebookScraper(config, factory);

                    return (scraper, platform);
                });
        }
    }
}