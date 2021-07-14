using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scraper.Net.Abstractions;

namespace Scraper.Net.Facebook.Tests
{
    [TestClass]
    public class FacebookScraperTests
    {
        private static FacebookScraper _scraper;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _scraper = new FacebookScraper(
                new FacebookScraperConfig());
        }

        [TestMethod]
        public async Task TestGetPostsAsync()
        {
            var user = new User("ayelet.benshaul.shaked", "Facebook");
            IEnumerable<Post> posts = await _scraper.GetPostsAsync(user);
            List<Post> list = posts.ToList();

            Assert.IsNotNull(list);
            CollectionAssert.AllItemsAreNotNull(list);
        }
    }
}