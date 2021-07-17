using System;
using System.Collections.Generic;
using System.Threading;

namespace Scraper.Net.Tests
{
    internal class MockScraper : IPlatformScraper
    {
        private readonly bool _ignoreCt;
        private readonly MockDelayScraper _scraper;

        public MockScraper(bool ignoreCt = false)
        {
            _ignoreCt = ignoreCt;
            _scraper = new MockDelayScraper(TimeSpan.Zero);
        }

        public IAsyncEnumerable<Post> GetPostsAsync(string id, CancellationToken ct = default)
        {
            return _ignoreCt 
                ? _scraper.GetPostsAsync(id) 
                : _scraper.GetPostsAsync(id, ct);
        }
    }
}