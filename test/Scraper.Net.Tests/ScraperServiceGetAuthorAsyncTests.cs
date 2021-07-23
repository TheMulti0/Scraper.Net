using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Tests
{
    [TestClass]
    public class ScraperServiceGetAuthorAsyncTests
    {
        [TestMethod]
        public async Task TestNoMatchingScraper()
        {
            var scraper = new ScraperService(
                new Dictionary<string, IPlatformScraper>(),
                new List<PostFilter>(),
                new List<IPostProcessor>(),
                NullLogger<ScraperService>.Instance);

            var id = "mockuser";

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await scraper.GetPostsAsync(id, "mock").ToListAsync());
        }
        
        [TestMethod]
        public async Task TestGetAuthorAsync()
        {
            IPlatformScraper mockScraper = new MockScraper();
            var scraper = new ScraperService(
                new Dictionary<string, IPlatformScraper> 
                {
                    {"mock", mockScraper}
                },
                new List<PostFilter>(),
                new List<IPostProcessor>(),
                NullLogger<ScraperService>.Instance);

            var id = "mockuser";
            
            var posts = await scraper.GetAuthorAsync(id, "mock");
            var p = await mockScraper.GetAuthorAsync(id);
            
            Assert.AreEqual(p, posts);
        }
    }
}