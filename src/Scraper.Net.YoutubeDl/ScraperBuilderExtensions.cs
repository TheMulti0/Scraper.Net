using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.YoutubeDl
{
    public static class ScraperBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="YoutubeDlPostProcessor"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <returns>A reference to this instance after the operation has completed</returns>
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