using System.Collections.Generic;
using System.Threading;

namespace Scraper.Net
{
    /// <summary>
    /// Processes posts
    /// </summary>
    public interface IPostProcessor
    {
        /// <summary>
        /// Process a single post into one or more posts asynchronously
        /// </summary>
        /// <param name="post">A post to process</param>
        /// <param name="platform">The platform of the post</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Async-enumerable of processed posts</returns>
        IAsyncEnumerable<Post> ProcessAsync(
            Post post,
            string platform,
            CancellationToken ct = default);
    }
}