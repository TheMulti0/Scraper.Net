using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlCssToImage.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Screenshot.Tests
{
    [TestClass]
    public class TwitterScreenshotterTests
    {
        private readonly HtmlCssToImageClient _client;
        private readonly TwitterScreenshotter _screenshotter;

        public TwitterScreenshotterTests()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<TwitterScreenshotterTests>()
                .Build();

            var credentials = new HtmlCssToImageCredentials(
                config["UserId"],
                config["ApiKey"]);

            _client = new HtmlCssToImageClient(credentials);
            _screenshotter = new TwitterScreenshotter(_client);
        }
        
        [TestMethod]
        public async Task TestTextTweetAsync()
        {
            await TestAsync("https://twitter.com/IsraelPolls/status/1362480543733014537");
        }
        
        [TestMethod]
        public async Task TestPhotoTweetAsync()
        {
            await TestAsync("https://twitter.com/Ayelet__Shaked/status/1363400109929684993");
        }
        
        [TestMethod]
        public async Task TestAlbumTweetAsync()
        {
            await TestAsync("https://twitter.com/yairlapid/status/1362479265762189313");
        }
        
        [TestMethod]
        public async Task TestReplyTweetAsync()
        {
            await TestAsync("https://twitter.com/kann_news/status/1363407092963508230");
        } 
        
        [TestMethod]
        public async Task TestReplyVideoTweetAsync()
        {
            await TestAsync("https://twitter.com/kann_news/status/1366432842503360519");
        }
        
        [TestMethod]
        public async Task TestReplyUrlTweetAsync()
        {
            await TestAsync("https://twitter.com/kann_news/status/1379654933851607043");
        }
        
        [TestMethod]
        public async Task TestReplyVideoUrlTweetAsync()
        {
            await TestAsync("https://twitter.com/kann_news/status/1366648731408490500");
        }
        
        [TestMethod]
        public async Task TestQuoteTweetAsync()
        {
            await TestAsync("https://twitter.com/bezalelsm/status/1363360010298875907");
        }
        
        private async Task TestAsync(string url)
        {
            string screenshotUrl = await _screenshotter.ScreenshotAsync(url);

            string screenshotId = screenshotUrl.Split("/").Last();

            Stream img = await _client.GetImageAsync(new GetImageRequest(screenshotId));
        
            Assert.IsNotNull(img);
        }
    }
}