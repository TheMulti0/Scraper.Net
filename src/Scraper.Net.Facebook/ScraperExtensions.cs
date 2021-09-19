using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Scraper.Net.Facebook
{
    public static class ScraperExtensions
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
            string platform = FacebookConstants.PlatformName)
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
        
        public static Task<Net.Author> GetFacebookAuthor(
            this IScraperService service,
            string id,
            CancellationToken ct = default)
            => service.GetAuthorAsync(id, FacebookConstants.PlatformName, ct);

        public static IAsyncEnumerable<Post> GetFacebookPosts(
            this IScraperService service,
            string id,
            CancellationToken ct = default)
            => service.GetPostsAsync(id, FacebookConstants.PlatformName, ct);
    }
}