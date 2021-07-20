using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Feeds.Tests
{
    [TestClass]
    public class FeedScraperGetPostsAsyncTests
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
            var posts = await FeedScraper.GetPostsAsync(url).ToListAsync();
            
            CollectionAssert.AllItemsAreNotNull(posts);

            foreach (Post post in posts)
            {
                Assert.IsNotNull(post.Content);
                Assert.IsNotNull(post.Url);
                Assert.IsNotNull(post.AuthorId);
            }
        }
    }
}