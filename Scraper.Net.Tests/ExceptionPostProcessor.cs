using System;
using System.Collections.Generic;

namespace Scraper.Net.Tests
{
    internal class ExceptionPostProcessor : IPostProcessor
    {
        public IAsyncEnumerable<Post> ProcessAsync(Post post)
        {
            throw new InvalidOperationException();
        }
    }
}