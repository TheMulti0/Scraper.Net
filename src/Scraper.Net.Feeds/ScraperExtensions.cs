using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Feeds
{
    public static class ScraperExtensions
    {

        /// <summary>
        /// Adds a <see cref="FeedsScraper"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="platform"></param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static ScraperBuilder AddFeeds(
            this ScraperBuilder builder,
            string platform = FeedsExtensions.PlatformName)
        {
            return builder
                .AddScraper(_ =>
                {
                    var scraper = new FeedsScraper();

                    return (scraper, platform);
                });
        }
        
        public static Task<Author> GetFeedsAuthor(
            this IScraperService service,
            string id,
            CancellationToken ct = default)
            => service.GetAuthorAsync(id, FeedsExtensions.PlatformName, ct);

        public static IAsyncEnumerable<Post> GetFeedsPosts(
            this IScraperService service,
            string id,
            CancellationToken ct = default)
            => service.GetPostsAsync(id, FeedsExtensions.PlatformName, ct);
    }
}