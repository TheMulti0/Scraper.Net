using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.Twitter
{
    public static class ScraperExtensions
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
            string platform = TwitterConstants.PlatformName)
        {
            return builder
                .AddScraper(provider =>
                {
                    config ??= provider.GetService<TwitterConfig>() ?? throw new ArgumentNullException(nameof(config));
                    
                    var scraper = ActivatorUtilities.CreateInstance<TwitterScraper>(provider, config);

                    return (scraper, platform);
                });
        }
        
        public static Task<Author> GetTwitterAuthor(
            this IScraperService service,
            string id,
            CancellationToken ct = default)
            => service.GetAuthorAsync(id, TwitterConstants.PlatformName, ct);

        public static IAsyncEnumerable<Post> GetTwitterPosts(
            this IScraperService service,
            string id,
            CancellationToken ct = default)
            => service.GetPostsAsync(id, TwitterConstants.PlatformName, ct);
    }
}