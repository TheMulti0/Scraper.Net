using System;
using System.Collections.Generic;
using System.Threading;

namespace Scraper.Net.Stream
{
    /// <summary>
    /// A stream of new posts, updated by periodic updates or manual update triggers
    /// </summary>
    public interface IPostStream : IObservable<Post>
    {
        /// <summary>
        /// Performs a manual update (scrape) for new posts,
        /// if a new batch is found, it will be pushed to the stream,
        /// and returned as an AsyncEnumerable
        /// </summary>
        /// <param name="ct"></param>
        /// <returns>Batch of new posts</returns>
        IAsyncEnumerable<Post> UpdateAsync(CancellationToken ct);
    }
}