using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Scraper.Net;

namespace PostsListener.Tests
{
    internal class MockScraperService : IScraperService
    {
        public Task<Author> GetAuthorAsync(string id, string platform, CancellationToken ct = new())
        {
            return Task.FromResult(new Author());
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(string id, string platform, [EnumeratorCancellation] CancellationToken ct = new())
        {
            yield return new Post();
        }
    }
}