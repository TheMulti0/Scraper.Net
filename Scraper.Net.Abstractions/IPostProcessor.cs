using System.Collections.Generic;

namespace Scraper.Net.Abstractions
{
    public interface IPostProcessor
    {
        IAsyncEnumerable<Post> ProcessAsync(Post post);
    }
}