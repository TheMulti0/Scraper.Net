using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.Facebook
{
    public static class ScraperBuilderExtensions
    {
        public static ScraperBuilder AddFacebook(
            this ScraperBuilder builder,
            FacebookConfig config = null,
            string platform = "facebook")
        {
            return builder
                .AddScraper(provider =>
                {
                    config ??= provider.GetService<FacebookConfig>() ?? new FacebookConfig();
                    var scraper = new FacebookScraper(config);

                    return (scraper, platform);
                });
        }
    }
}