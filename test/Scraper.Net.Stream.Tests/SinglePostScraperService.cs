using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Stream.Tests
{
    internal class SinglePostScraperService : IScraperService
    {
        private int _counter = 0;
        
        public Task<Author> GetAuthorAsync(string id, string platform, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(string id, string platform, CancellationToken ct = default)
        {
            _counter++;
            
            switch (id)
            {
                case "noid":
                    throw new IdNotFoundException(id);
                case "onetime" when _counter < 1:
                    throw new InvalidOperationException();
                default:
                    yield return new Post
                    {
                        Content = _counter.ToString()
                    };
                    break;
            }
        }
    }
}