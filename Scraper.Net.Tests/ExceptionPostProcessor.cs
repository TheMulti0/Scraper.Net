﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Scraper.Net.Tests
{
    internal class ExceptionPostProcessor : IPostProcessor
    {
        public IAsyncEnumerable<Post> ProcessAsync(Post post, CancellationToken ct = default)
        {
            throw new InvalidOperationException();
        }
    }
}