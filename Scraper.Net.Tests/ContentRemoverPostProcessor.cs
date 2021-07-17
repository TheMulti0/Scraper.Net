using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Scraper.Net.Tests
{
    internal class ContentRemoverPostProcessor : IPostProcessor
    {
        public async IAsyncEnumerable<Post> ProcessAsync(
            Post post,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            yield return post with { Content = null };
        }
    }
}