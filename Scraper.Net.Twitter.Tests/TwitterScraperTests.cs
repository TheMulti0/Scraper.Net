using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scraper.Net.Abstractions;
using Tweetinvi.Models;

namespace Scraper.Net.Twitter.Tests
{
    [TestClass]
    public class TwitterScraperTests
    {
        private readonly TwitterScraper _scraper;
        private readonly TextCleaner _textCleaner;
        private readonly MediaItemsExtractor _mediaItemsExtractor;

        public TwitterScraperTests()
        {
            IConfigurationRoot rootConfig = new ConfigurationBuilder()
                .AddUserSecrets<TwitterScraperTests>()
                .Build();

            var config = rootConfig.Get<TwitterScraperConfig>();
            
            _scraper = new TwitterScraper(config);
            _textCleaner = new TextCleaner();
            _mediaItemsExtractor = new MediaItemsExtractor();
        }

        [TestMethod]
        public async Task TestTheMulti0()
        {
            var user = new User("themulti0", "twitter");
            
            IEnumerable<Post> posts = await _scraper.GetPostsAsync(user);
            List<Post> list = posts.ToList();
            
            Assert.IsNotNull(list);
            CollectionAssert.AllItemsAreNotNull(list);
        }
        
        [TestMethod]
        public void TestCleanTextWithShortenedUrl()
        {
            const string url = "https://t.co/Po1PsWNB49";
            
            var expanded = _textCleaner.CleanText(url);
            
            Assert.IsFalse(expanded.Contains(url));
        }
        
        [TestMethod]
        public void TestCleanTextWithPictureShortenedUrl()
        {
            const string url = "https://pic.twitter.com/epqvVYkWKC";
            
            var expanded = _textCleaner.CleanText(url);
            
            Assert.IsFalse(expanded.Contains(url));
        }

        [TestMethod]
        public async Task TestPhotoExtraction()
        {
            const long tweetId = 1361968093686333440;
            
            ITweet tweet = await _scraper.TwitterClient.Tweets.GetTweetAsync(tweetId);

            IEnumerable<IMediaItem> media = _mediaItemsExtractor.ExtractMediaItems(tweet);
            
            Assert.IsTrue(media.Any(m => m is PhotoItem));
        }

        [TestMethod]
        public async Task TestVideoExtraction()
        {
            const long tweetId = 1361968818323664899;
            
            ITweet tweet = await _scraper.TwitterClient.Tweets.GetTweetAsync(tweetId);

            IEnumerable<IMediaItem> media = _mediaItemsExtractor.ExtractMediaItems(tweet);
            
            Assert.IsTrue(media.Any(m => m is VideoItem));
        }
    }
}