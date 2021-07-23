using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net
{
    /// <summary>
    /// Provides multi-platform scraping capabilities
    /// </summary>
    public interface IScraperService
    {
        /// <summary>
        /// Finds an author in a platform by id
        /// </summary>
        /// <param name="id">Author id</param>
        /// <param name="platform">Platform to query</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task of an Author</returns>
        Task<Author> GetAuthorAsync(
            string id,
            string platform,
            CancellationToken ct = default);
        
        /// <summary>
        /// Finds posts in a platform by author id
        /// </summary>
        /// <param name="id">Author id</param>
        /// <param name="platform">Platform to query</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>A finite async-enumerable of the posts</returns>
        IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            string platform,
            CancellationToken ct = default);
    }
}