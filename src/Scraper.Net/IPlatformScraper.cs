using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net
{
    public interface IPlatformScraper
    {
        Task<Author> GetAuthorAsync(
            string id,
            CancellationToken ct = default);
        
        IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            CancellationToken ct = default);
    }
}