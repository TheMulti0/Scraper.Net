using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Tests
{
    internal class MockDelayScraper : IPlatformScraper
    {
        private readonly TimeSpan _delay;

        public MockDelayScraper(TimeSpan delay)
        {
            _delay = delay;
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            for (var i = 0; i < 5; i++)
            {
                await Task.Delay(_delay, ct);

                yield return new Post
                {
                    Content = i.ToString()
                };
            }
        }
    }
}