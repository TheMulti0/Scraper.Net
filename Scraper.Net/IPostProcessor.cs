using System.Collections.Generic;

namespace Scraper.Net
{
    public interface IPostProcessor
    {
        IAsyncEnumerable<Post> ProcessAsync(Post post);
    }
}