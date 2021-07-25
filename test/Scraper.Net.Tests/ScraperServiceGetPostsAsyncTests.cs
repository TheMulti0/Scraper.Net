using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Tests
{
    [TestClass]
    public class ScraperServiceGetPostsAsyncTests
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
        public async Task TestPostDelivery()
        {
            var mockScraper = new MockScraper();
            var scraper = new ScraperService(
                new Dictionary<string, IPlatformScraper> 
                {
                    {"mock", mockScraper}
                },
                new List<PostFilter>(),
                new List<IPostProcessor>(),
                NullLogger<ScraperService>.Instance);

            var id = "mockuser";
            
            var posts = await scraper.GetPostsAsync(id, "mock").ToListAsync();
            var p = await mockScraper.GetPostsAsync(id).ToListAsync();
            
            CollectionAssert.AreEqual(p, posts);
        }
        
        [TestMethod]
        public async Task TestPostFilter()
        {
            var mockScraper = new MockScraper();
            var scraper = new ScraperService(
                new Dictionary<string, IPlatformScraper> 
                {
                    {"mock", mockScraper}
                },
                new PostFilter[]
                {
                    (post, platform) => false
                },
                new List<IPostProcessor>(),
                NullLogger<ScraperService>.Instance);

            var id = "mockuser";
            
            var posts = await scraper.GetPostsAsync(id, "mock").ToListAsync();
            
            Assert.IsTrue(!posts.Any());
        }

        [TestMethod]
        public async Task TestPostProcessing()
        {
            var scraper = new ScraperService(
                new Dictionary<string, IPlatformScraper> 
                {
                    {"mock", new MockScraper()}
                },
                new List<PostFilter>(),
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
        
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(100)]
        [DataRow(1000)]
        public async Task TestPostProcessorExceptionCatch(int delayMs)
        {
            var delay = TimeSpan.FromMilliseconds(delayMs);
            
            var scraper = new ScraperService(
                new Dictionary<string, IPlatformScraper> 
                {
                    {"mock", new MockScraper()}
                },
                new List<PostFilter>(),
                new List<IPostProcessor>
                {
                    new ExceptionDelayPostProcessor(delay)
                },
                NullLogger<ScraperService>.Instance);

            await scraper.GetPostsAsync("mockuser", "mock").ToListAsync();
        }
    }
}