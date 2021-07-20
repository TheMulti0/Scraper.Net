using HtmlCssToImage.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.Screenshot
{
    public static class ScreenshotterBuilderExtensions
    {
        public static ScreenshotterBuilder AddTwitter(
            this ScreenshotterBuilder builder,
            string platform = "twitter")
        {
            return builder
                .AddScreenshotter(provider =>
                {
                    var screenshotter = new TwitterScreenshotter(provider.GetService<IHtmlCssToImageClient>());

                    return (screenshotter, platform);
                });
        }
    }
}