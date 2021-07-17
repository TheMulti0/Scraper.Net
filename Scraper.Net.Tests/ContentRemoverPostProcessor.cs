using System.Collections.Generic;

namespace Scraper.Net.Tests
{
    internal class ContentRemoverPostProcessor : IPostProcessor
    {
        public async IAsyncEnumerable<Post> ProcessAsync(Post post)
        {
            yield return post with { Content = null };
        }
    }
}