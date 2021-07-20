using System;
using System.Collections.Generic;
using System.Linq;
using HtmlCssToImage.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.Screenshot
{
    public static class ScraperBuilderExtensions
    {
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