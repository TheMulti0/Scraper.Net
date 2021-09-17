using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Youtube.Tests
{
    [TestClass]
    public class YoutubeScraperTests
    {
        private readonly YoutubeScraper _youtubeScraper;

        public YoutubeScraperTests()
        {
            IConfigurationRoot rootConfig = new ConfigurationBuilder()
                .AddUserSecrets<YoutubeScraperTests>()
                .Build();
            
            var config = rootConfig.Get<YoutubeScraperConfig>();

            _youtubeScraper = new YoutubeScraper(config);
        }
        
        [TestMethod]
        public async Task TestGetAuthorAsync()
        {
            var author = await _youtubeScraper.GetAuthorAsync("UC4x7LYSzgGH-TMKc9J8pwgQ");
            
            Assert.IsNotNull(author);
        }

        [TestMethod]
        public async Task TestGetPostsAsync()
        {
            List<Post> posts = await _youtubeScraper.GetPostsAsync("UC4x7LYSzgGH-TMKc9J8pwgQ")
                .ToListAsync();
            
            Assert.IsTrue(posts.Any());
        }
    }
}