using HtmlCssToImage.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.Screenshot
{
    public static class ScreenshotterBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="TwitterScreenshotter"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="platform"></param>
        /// <returns>A reference to this instance after the operation has completed</returns>
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