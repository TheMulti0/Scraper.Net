using System.Threading;
using System.Threading.Tasks;
using Scraper.Net;

namespace PostsListener
{
    public class PostUrlFilter
    {
        private readonly IPostUrlsPersistence _persistence;

        public PostUrlFilter(IPostUrlsPersistence persistence)
        {
            _persistence = persistence;
        }

        public async Task<bool> FilterAsync(
            Post post,
            CancellationToken ct)
        {
            if (post.Url == null || await _persistence.ExistsAsync(post.Url, ct))
            {
                return false;
            }

            await _persistence.AddAsync(post.Url, ct);

            return true;
        }
    }
}