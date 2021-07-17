using System.Collections.Generic;
using System.Threading;

namespace Scraper.Net
{
    public interface IPlatformScraper
    {
        IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            CancellationToken ct = default);
    }
}