using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scraper.Net.Tests
{
    internal class MockScraper : IPlatformScraper
    {
        public Task<IEnumerable<Post>> GetPostsAsync(string id)
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

            return Task.FromResult(posts);
        }
    }
}