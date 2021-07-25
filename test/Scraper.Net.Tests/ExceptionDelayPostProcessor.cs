using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Tests
{
    internal class ExceptionDelayPostProcessor : IPostProcessor
    {
        private readonly TimeSpan _delay;

        public ExceptionDelayPostProcessor(TimeSpan delay)
        {
            _delay = delay;
        }

        public async IAsyncEnumerable<Post> ProcessAsync(
            Post post,
            string platform,
            CancellationToken ct = default)
        {
            await Task.Delay(_delay, ct);
            
            throw new InvalidOperationException();

            yield break;
        }
    }
}