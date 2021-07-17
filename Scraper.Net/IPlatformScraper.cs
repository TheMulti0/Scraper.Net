using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net
{
    public interface IPlatformScraper
    {
        Task<IEnumerable<Post>> GetPostsAsync(
            string id,
            CancellationToken ct = default);
    }
}