using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Twitter.Tests
{
    [TestClass]
    public class UserScraperTests
    {
        private readonly TwitterScraper _scraper;

        public UserScraperTests()
        {
            IConfigurationRoot rootConfig = new ConfigurationBuilder()
                .AddUserSecrets<UserScraperTests>()
                .Build();

            var config = rootConfig.Get<TwitterConfig>();

            _scraper = new TwitterScraper(config);
        }

        [TestMethod]
        public async Task TestTheMulti0()
        {
            const string user = "themulti0";

            Author author = await _scraper.GetAuthorAsync(user);
            
            Assert.IsNotNull(author);
        }
    }
}