using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.Youtube
{
    public static class ScraperExtensions
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
            string platform = YoutubeConstants.PlatformName)
        {
            return builder
                .AddScraper(provider =>
                {
                    config ??= provider.GetService<YoutubeConfig>() ?? throw new ArgumentNullException(nameof(config));
                    
                    var scraper = ActivatorUtilities.CreateInstance<YoutubeScraper>(provider, config);

                    return (scraper, platform);
                });
        }
        
        public static Task<Author> GetYoutubeAuthor(
            this IScraperService service,
            string id,
            CancellationToken ct = default)
            => service.GetAuthorAsync(id, YoutubeConstants.PlatformName, ct);

        public static IAsyncEnumerable<Post> GetYoutubePosts(
            this IScraperService service,
            string id,
            CancellationToken ct = default)
            => service.GetPostsAsync(id, YoutubeConstants.PlatformName, ct);
    }
}