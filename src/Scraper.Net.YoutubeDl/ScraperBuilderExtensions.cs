using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.YoutubeDl
{
    public static class ScraperBuilderExtensions
    {
        public static ScraperBuilder AddYoutubeDl(
            this ScraperBuilder builder,
            YoutubeDlConfig config = null)
        {
            return builder
                .AddPostProcessor(provider =>
                {
                    config ??= provider.GetService<YoutubeDlConfig>() ?? new YoutubeDlConfig();
                    
                    return new YoutubeDlPostProcessor(config);
                });
        }
    }
}