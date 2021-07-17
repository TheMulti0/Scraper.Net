using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Tests
{
    internal class MockDelayPostProcessor : IPostProcessor
    {
        private readonly TimeSpan _delay;
        
        public MockDelayPostProcessor(TimeSpan delay)
        {
            _delay = delay;
        }
        
        public async IAsyncEnumerable<Post> ProcessAsync(
            Post post,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            await Task.Delay(_delay, ct);
            
            yield return post with { Content = null };
        }
    }
}