using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Screenshot.Tests
{
    [TestClass]
    public class ScraperServiceTests
    {
        [TestMethod]
        public async Task TestNoMatchingScreenshotter()
        {
            var screenshot = new ScreenshotPostProcessor(
                false,
                new Dictionary<string, IPlatformScreenshotter>());

            var id = "mockuser";

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await screenshot.ProcessAsync(new Post(), string.Empty).ToListAsync());
        }
        
        [TestMethod]
        public async Task TestScreenshot()
        {
            var screenshot = new ScreenshotPostProcessor(
                false,
                new Dictionary<string, IPlatformScreenshotter>
                {
                    {"mock", new MockScreenshotter()}
                });

            var myurl = "myurl";
            var post = new Post
            {
                Url = myurl
            };

            List<Post> posts = await screenshot.ProcessAsync(post, "mock").ToListAsync();
            Assert.AreEqual(1, posts.Count);

            var processedPost = posts.First();
            Assert.AreEqual(1, processedPost.MediaItems.Count());
            Assert.AreEqual(myurl, processedPost.MediaItems.First().Url);
        }
    }
}