using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tweetinvi.Models;

namespace Scraper.Net.Twitter.Tests
{
    [TestClass]
    public class TwitterScraperTests
    {
        private readonly TwitterScraper _scraper;
        private readonly TweetScraper _tweetScraper;
        private readonly TextCleaner _textCleaner;
        private readonly MediaItemsExtractor _mediaItemsExtractor;

        public TwitterScraperTests()
        {
            IConfigurationRoot rootConfig = new ConfigurationBuilder()
                .AddUserSecrets<TwitterScraperTests>()
                .Build();

            var config = rootConfig.Get<TwitterConfig>();
            
            _scraper = new TwitterScraper(config);
            _tweetScraper = new TweetScraper(config);
            _textCleaner = new TextCleaner();
            _mediaItemsExtractor = new MediaItemsExtractor();
        }

        [TestMethod]
        public async Task TestTheMulti0()
        {
            const string user = "themulti0";

            List<Post> posts = await _scraper.GetPostsAsync(user).ToListAsync();
            
            Assert.IsNotNull(posts);
            CollectionAssert.AllItemsAreNotNull(posts);
        }
        
        [TestMethod]
        public async Task TestCleanTextWithShortenedUrl()
        {
            const string url = "https://t.co/Po1PsWNB49";
            
            var expanded = await _textCleaner.CleanTextAsync(url);
            
            Assert.IsFalse(expanded.Contains(url));
        }
        
        [TestMethod]
        public async Task TestCleanTextWithPictureShortenedUrl()
        {
            const string url = "https://pic.twitter.com/epqvVYkWKC";
            
            var expanded = await _textCleaner.CleanTextAsync(url);
            
            Assert.IsFalse(expanded.Contains(url));
        }

        [TestMethod]
        public async Task TestPhotoExtraction()
        {
            const long tweetId = 1361968093686333440;
            
            ITweet tweet = await _tweetScraper.TwitterClient.Tweets.GetTweetAsync(tweetId);

            IEnumerable<IMediaItem> media = _mediaItemsExtractor.ExtractMediaItems(tweet);
            
            Assert.IsTrue(media.Any(m => m is PhotoItem));
        }

        [TestMethod]
        public async Task TestVideoExtraction()
        {
            const long tweetId = 1361968818323664899;
            
            ITweet tweet = await _tweetScraper.TwitterClient.Tweets.GetTweetAsync(tweetId);

            IEnumerable<IMediaItem> media = _mediaItemsExtractor.ExtractMediaItems(tweet);
            
            Assert.IsTrue(media.Any(m => m is VideoItem));
        }
    }
}