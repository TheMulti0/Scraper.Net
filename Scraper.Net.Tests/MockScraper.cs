using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Scraper.Net.Tests
{
    internal class MockScraper : IPlatformScraper
    {
        public IAsyncEnumerable<Post> GetPostsAsync(string id, CancellationToken ct =default)
        {
            IEnumerable<Post> posts = new []
            {
                new Post
                {
                    Content = "Mock1"
                },
                new Post
                {
                    Content = "Mock2"
                },
                new Post
                {
                    Content = "Mock3"
                }
            };

            return posts.ToAsyncEnumerable();
        }
    }
}