using System;
using System.Collections.Generic;
using System.Linq;
using HtmlCssToImage.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.Screenshot
{
    public static class ScraperBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="ScreenshotPostProcessor"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="builderAction">Screenshot-related services factory</param>
        /// <param name="config"></param>
        /// <param name="keepReceivedPost">If set to true, the post processor will return the original post in addition to the processed one</param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static ScraperBuilder AddScreenshot(
            this ScraperBuilder builder,
            Action<ScreenshotterBuilder> builderAction,
            HtmlCssToImageCredentials config = null,
            bool keepReceivedPost = false)
        {
            IHtmlCssToImageClient CreateHtmlCssToImageClient(IServiceProvider provider)
            {
                config ??= provider.GetService<HtmlCssToImageCredentials>() ?? throw new ArgumentNullException(nameof(config));

                return new HtmlCssToImageClient(config);
            }

            builder.Services.AddSingleton(CreateHtmlCssToImageClient);
            
            builderAction(new ScreenshotterBuilder(builder.Services));

            IPostProcessor CreateScreenshotPostProcessor(IServiceProvider provider)
            {
                Dictionary<string, IPlatformScreenshotter> platformScreenshotters = provider
                    .GetServices<RegisteredPlatformScreenshotter>()
                    .ToDictionary(r => r.Platform, r => r.Screenshotter);

                return new ScreenshotPostProcessor(keepReceivedPost, platformScreenshotters);
            }

            return builder.AddPostProcessor(CreateScreenshotPostProcessor);
        }
    }
}