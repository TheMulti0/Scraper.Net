using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tweetinvi;
using Tweetinvi.Models;

namespace Scraper.Net.Twitter.Tests
{
    [TestClass]
    public class TwitterScraperGetPostsAsyncTests
    {
        private readonly ITwitterClient _twitterClient;
        private readonly TwitterScraper _scraper;
        private readonly TextCleaner _textCleaner;
        private readonly MediaItemsExtractor _mediaItemsExtractor;

        public TwitterScraperGetPostsAsyncTests()
        {
            IConfigurationRoot rootConfig = new ConfigurationBuilder()
                .AddUserSecrets<TwitterScraperGetPostsAsyncTests>()
                .Build();

            var config = rootConfig.Get<TwitterConfig>();

            _twitterClient = TwitterClientFactory.CreateAsync(config).Result;
            _scraper = new TwitterScraper(config);
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
        public async Task TestNotFound()
        {
            const string user = "randomuseridthatdoesntexist123456789453123456789";

            await Assert.ThrowsExceptionAsync<IdNotFoundException>(async () => await _scraper.GetPostsAsync(user).ToListAsync());
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
            
            ITweet tweet = await _twitterClient.Tweets.GetTweetAsync(tweetId);

            IEnumerable<IMediaItem> media = _mediaItemsExtractor.ExtractMediaItems(tweet);
            
            Assert.IsTrue(media.Any(m => m is PhotoItem));
        }

        [TestMethod]
        public async Task TestVideoExtraction()
        {
            const long tweetId = 1361968818323664899;
            
            ITweet tweet = await _twitterClient.Tweets.GetTweetAsync(tweetId);

            IEnumerable<IMediaItem> media = _mediaItemsExtractor.ExtractMediaItems(tweet);
            
            Assert.IsTrue(media.Any(m => m is VideoItem));
        }
    }
}