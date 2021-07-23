using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Feeds.Tests
{
    [TestClass]
    public class FeedsScraperGetAuthorAsyncTests
    {
        private static readonly FeedsScraper FeedsScraper = new();
        
        [TestMethod]
        public async Task TestMako()
        {
            await Test("https://rcs.mako.co.il/rss/news-military.xml");
        }

        [TestMethod]
        public async Task TestYnet()
        {
            await Test("http://www.ynet.co.il/Integration/StoryRss2.xml");
        }

        [TestMethod]
        public async Task TestNotFound()
        {
            await TestFailure<IdNotFoundException>("somenotaccesibleurl"); // Try as file
            await TestFailure<IdNotFoundException>("https://somenotaccesibleurl"); // Try url
        }

        private static async Task Test(string url)
        {
            var author = await FeedsScraper.GetAuthorAsync(url);

            Assert.IsNotNull(author.Id);
        }

        private static Task TestFailure<T>(string url) where T : Exception
        {
            return Assert.ThrowsExceptionAsync<T>(async () => await FeedsScraper.GetAuthorAsync(url));
        }
    }
}