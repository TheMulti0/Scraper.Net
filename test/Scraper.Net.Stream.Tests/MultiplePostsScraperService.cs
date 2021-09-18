using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Stream.Tests
{
    internal class MultiplePostsScraperService : IScraperService
    {
        public Task<Author> GetAuthorAsync(string id, string platform, CancellationToken ct = default) => throw new NotImplementedException();

        public async IAsyncEnumerable<Post> GetPostsAsync(string id, string platform, [EnumeratorCancellation] CancellationToken ct = default)
        {
            DateTime creationDate = DateTime.Today;
            yield return new Post
            {
                CreationDate = creationDate
            };
            yield return new Post
            {
                CreationDate = creationDate.AddSeconds(1)
            };
        }
    }
}