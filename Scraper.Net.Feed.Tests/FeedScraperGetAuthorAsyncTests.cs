using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Feed.Tests
{
    [TestClass]
    public class FeedScraperGetAuthorAsyncTests
    {
        private static readonly FeedScraper FeedScraper = new();
        
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

        private static async Task Test(string url)
        {
            var author = await FeedScraper.GetAuthorAsync(url);

            Assert.IsNotNull(author.Id);
        }
    }
}