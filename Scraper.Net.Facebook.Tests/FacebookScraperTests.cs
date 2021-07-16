using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Facebook.Tests
{
    [TestClass]
    public class FacebookScraperTests
    {
        private static FacebookScraper _scraper;
        private readonly User _user = new("ayelet.benshaul.shaked", "Facebook");

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _scraper = new FacebookScraper(
                new FacebookScraperConfig());
        }

        [TestMethod]
        public async Task TestGetPostsAsync()
        {
            IEnumerable<Post> posts = await _scraper.GetPostsAsync(_user);
            List<Post> list = posts.ToList();

            Assert.IsNotNull(list);
            CollectionAssert.AllItemsAreNotNull(list);
        }
    }
}