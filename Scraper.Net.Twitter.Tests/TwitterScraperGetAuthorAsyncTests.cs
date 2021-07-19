using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Twitter.Tests
{
    [TestClass]
    public class TwitterScraperGetAuthorAsyncTests
    {
        private readonly TwitterScraper _scraper;

        public TwitterScraperGetAuthorAsyncTests()
        {
            IConfigurationRoot rootConfig = new ConfigurationBuilder()
                .AddUserSecrets<TwitterScraperGetAuthorAsyncTests>()
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
        
        [TestMethod]
        public async Task TestNotFound()
        {
            const string user = "randomuseridthatdoesntexist123456789453123456789";

            await Assert.ThrowsExceptionAsync<IdNotFoundException>(async () => await _scraper.GetAuthorAsync(user));
        }
    }
}