using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Stream.Tests
{
    internal class SinglePostScraperService : IScraperService
    {
        public Task<Author> GetAuthorAsync(string id, string platform, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Post> GetPostsAsync(string id, string platform, CancellationToken ct = default)
        {
            return AsyncEnumerable.Repeat(new Post(), 1);
        }
    }
}