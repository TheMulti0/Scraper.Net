using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net
{
    /// <summary>
    /// Scrapes a specific platform 
    /// </summary>
    public interface IPlatformScraper
    {
        /// <summary>
        /// Finds an author by id
        /// </summary>
        /// <param name="id">Author id</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task of an Author</returns>
        Task<Author> GetAuthorAsync(
            string id,
            CancellationToken ct = default);
        
        /// <summary>
        /// Finds posts by author id
        /// </summary>
        /// <param name="id">Author id</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Async-enumerable of posts</returns>
        IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            CancellationToken ct = default);
    }
}