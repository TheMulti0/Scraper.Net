using System.Collections.Generic;
using System.Threading;

namespace Scraper.Net
{
    public interface IPostProcessor
    {
        IAsyncEnumerable<Post> ProcessAsync(
            Post post,
            CancellationToken ct = default);
    }
}