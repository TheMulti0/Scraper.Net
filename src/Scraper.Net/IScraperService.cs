using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net
{
    public interface IScraperService
    {
        Task<Author> GetAuthorAsync(
            string id,
            string platform,
            CancellationToken ct = default);
        
        IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            string platform,
            CancellationToken ct = default);
    }
}