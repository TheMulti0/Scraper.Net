using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Twitter.Tests
{
    [TestClass]
    public class UrlExpanderTests
    {
        private readonly UrlExpander _expander;

        public UrlExpanderTests()
        {
            _expander = new UrlExpander();
        }

        [TestMethod]
        public async Task TestTwitterShortenedUrl()
        {
            const string url = "https://t.co/QOSO1d4kY3";
            
            var expanded = await _expander.ExpandAsync(url);
            
            Assert.IsNotNull(expanded);
            Assert.AreNotEqual(url, expanded);
            Assert.AreEqual("https://twitter.com/moran_ynet/status/1333356000351576064", expanded);
        }
        
        [TestMethod]
        public async Task TestTwitterShortenedFacebookPostUrl()
        {
            const string url = "https://t.co/Po1PsWNB49";
            
            var expanded = await _expander.ExpandAsync(url);
            
            Assert.IsNotNull(expanded);
            Assert.AreNotEqual(url, expanded);
            Assert.AreEqual("https://www.facebook.com/396697410351933/posts/3669190506435924/?d=n", expanded);
        }
    }
}