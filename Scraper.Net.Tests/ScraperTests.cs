using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Tests
{
    [TestClass]
    public class ScraperTests
    {
        [TestMethod]
        public async Task TestPostDelivery()
        {
            var mockScraper = new MockScraper();
            var scraper = new Scraper(
                new Dictionary<string, IPlatformScraper> 
                {
                    {"mock", mockScraper}
                },
                new List<IPostProcessor>());

            var id = "mockuser";
            
            var posts = await scraper.GetPostsAsync(id, "mock").ToListAsync();
            var p = (await mockScraper.GetPostsAsync(id)).ToList();
            
            CollectionAssert.AreEqual(p, posts);
        }

        [TestMethod]
        public async Task TestPostProcessing()
        {
            var scraper = new Scraper(
                new Dictionary<string, IPlatformScraper> 
                {
                    {"mock", new MockScraper()}
                },
                new List<IPostProcessor>
                {
                    new ContentRemoverPostProcessor()
                });

            var posts = await scraper.GetPostsAsync("mockuser", "mock").ToListAsync();

            foreach (Post post in posts)
            {
                Assert.IsNull(post.Content);
            }
        }
        
        [TestMethod]
        public async Task TestPostProcessorExceptionCatch()
        {
            var scraper = new Scraper(
                new Dictionary<string, IPlatformScraper> 
                {
                    {"mock", new MockScraper()}
                },
                new List<IPostProcessor>
                {
                    new ExceptionPostProcessor()
                });

            await scraper.GetPostsAsync("mockuser", "mock").ToListAsync();
        }
    }
}