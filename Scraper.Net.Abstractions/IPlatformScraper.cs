using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scraper.Net.Abstractions
{
    public interface IPlatformScraper
    {
        Task<IEnumerable<Post>> GetPostsAsync(User user);
    }
}