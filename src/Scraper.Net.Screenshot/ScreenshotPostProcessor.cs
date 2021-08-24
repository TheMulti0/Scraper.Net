using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Scraper.Net.Screenshot
{
    public class ScreenshotPostProcessor : IPostProcessor
    {
        private readonly bool _keepReceivedPost;
        private readonly Dictionary<string, IPlatformScreenshotter> _platformScreenshotters;

        public ScreenshotPostProcessor(
            bool keepReceivedPost,
            Dictionary<string, IPlatformScreenshotter> platformScreenshotters)
        {
            _keepReceivedPost = keepReceivedPost;
            _platformScreenshotters = platformScreenshotters;
        }

        public async IAsyncEnumerable<Post> ProcessAsync(
            Post post,
            string platform,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            if (_keepReceivedPost)
            {
                yield return post;
            }

            IPlatformScreenshotter screenshotter = GetScreenshotter(platform);

            string screenshotUrl = await screenshotter.ScreenshotAsync(post.Url);
            var screenshot = new ScreenshotItem(screenshotUrl);
            
            yield return post with { MediaItems = new [] { screenshot } };
        }
        
        private IPlatformScreenshotter GetScreenshotter(string platform)
        {
            if (!_platformScreenshotters.ContainsKey(platform))
            {
                throw new ArgumentException($"No screenshotter found for platform {platform}");
            }

            return _platformScreenshotters[platform];
        }
    }
}