using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.YoutubeDl.Tests
{
    [TestClass]
    public class YoutubeDlPostProcessorTests
    {
        private readonly YoutubeDlPostProcessor _youtubeDl = new(new YoutubeDlConfig());
        
        [DataTestMethod]
        [DataRow("https://facebook.com/hebpmo/posts/4293743697354072")]
        [DataRow("https://facebook.com/shirlypinto89/posts/819315905613113")]
        [DataRow("https://facebook.com/naftalibennett/posts/4317128274975474")]
        [DataRow("https://facebook.com/%d7%a0%d7%99%d7%a8-%d7%90%d7%95%d7%a8%d7%91%d7%9a-nir-orbach-639391266187801/posts/3921940434599518")]
        public async Task TestFacebook(string url)
        {
            await Test(url);
        }
        
        [DataTestMethod]
        [DataRow("https://www.youtube.com/watch?v=Xtvqhyo5c74")]
        [DataRow("https://www.youtube.com/watch?v=ZP3ju1Fdgdw")]
        [DataRow("https://www.youtube.com/watch?v=nj-aEOKDIzE")]
        public async Task TestYoutube(string url)
        {
            await Test(url);
        }

        private async Task Test(string url)
        {
            try
            {
                var originalPost = new Post
                {
                    Url = url
                };

                Post post = await _youtubeDl.ProcessAsync(originalPost, "").FirstOrDefaultAsync();

                Assert.IsNotNull(post);

                IMediaItem video = post.MediaItems.FirstOrDefault(item => item is VideoItem);

                Assert.IsNotNull(video);
                Assert.IsNotNull(video.Url);
            }
            catch (LoginRequiredException)
            {
                Assert.Inconclusive("Login required");
            }
        }
    }
}