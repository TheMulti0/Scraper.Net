using System.Threading.Tasks;

namespace Scraper.Net.Screenshot
{
    public interface IPlatformScreenshotter
    {
        Task<string> ScreenshotAsync(string url);
    }
}