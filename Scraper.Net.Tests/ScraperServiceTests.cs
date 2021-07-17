using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Tests
{
    [TestClass]
    public class ScraperServiceTests
    {
        [TestMethod]
        public async Task TestNoMatchingScraper()
        {
            var scraper = new ScraperService(
                new Dictionary<string, IPlatformScraper>(),
                new List<IPostProcessor>(),
                NullLogger<ScraperService>.Instance);

            var id = "mockuser";

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await scraper.GetPostsAsync(id, "mock").ToListAsync());
        }
        
        [TestMethod]
        public async Task TestPostDelivery()
        {
            var mockScraper = new MockScraper();
            var scraper = new ScraperService(
                new Dictionary<string, IPlatformScraper> 
                {
                    {"mock", mockScraper}
                },
                new List<IPostProcessor>(),
                NullLogger<ScraperService>.Instance);

            var id = "mockuser";
            
            var posts = await scraper.GetPostsAsync(id, "mock").ToListAsync();
            var p = await mockScraper.GetPostsAsync(id).ToListAsync();
            
            CollectionAssert.AreEqual(p, posts);
        }

        [TestMethod]
        public async Task TestPostProcessing()
        {
            var scraper = new ScraperService(
                new Dictionary<string, IPlatformScraper> 
                {
                    {"mock", new MockScraper()}
                },
                new List<IPostProcessor>
                {
                    new ContentRemoverPostProcessor()
                },
                NullLogger<ScraperService>.Instance);

            var posts = await scraper.GetPostsAsync("mockuser", "mock").ToListAsync();

            foreach (Post post in posts)
            {
                Assert.IsNull(post.Content);
            }
        }
        
        [TestMethod]
        public async Task TestPostProcessorExceptionCatch()
        {
            var scraper = new ScraperService(
                new Dictionary<string, IPlatformScraper> 
                {
                    {"mock", new MockScraper()}
                },
                new List<IPostProcessor>
                {
                    new ExceptionPostProcessor()
                },
                NullLogger<ScraperService>.Instance);

            await scraper.GetPostsAsync("mockuser", "mock").ToListAsync();
        }
    }
}