using System.Collections.Generic;
using System.Threading;

namespace Scraper.Net
{
    public interface IScraperService
    {
        IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            string platform,
            CancellationToken ct = default);
    }
}