using System.Threading.Tasks;

namespace Scraper.Net.Screenshot.Tests
{
    internal class MockScreenshotter : IPlatformScreenshotter
    {
        public Task<string> ScreenshotAsync(string url)
        {
            return Task.FromResult(url);
        }
    }
}