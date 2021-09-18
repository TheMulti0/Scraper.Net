using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace Scraper.Net.YoutubeDl
{
    /// <summary>
    /// <see cref="IPostProcessor"/> for downloading high-quality videos.
    /// The scraping engine is powered by YoutubeDLSharp
    /// </summary>
    public class YoutubeDlPostProcessor : IPostProcessor
    {
        private readonly bool _keepReceivedPost;
        private readonly VideoExtractor _videoExtractor;

        public YoutubeDlPostProcessor(
            YoutubeDlConfig config)
        {
            _keepReceivedPost = config.KeepReceivedPost;
            
            var youtubeDl = new YoutubeDL(config.DegreeOfConcurrency)
            {
                YoutubeDLPath = config.YoutubeDlPath
            };

            _videoExtractor = new VideoExtractor(youtubeDl, config.OverrideOptions);
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
            
            if (!post.MediaItems.Any(item => item is VideoItem))
            {
                yield break;
            }
            
            VideoItem videoItem = await _videoExtractor.ExtractAsync(post.Url, ct);

            IEnumerable<IMediaItem> mediaItems = post.MediaItems
                .Where(item => item is not VideoItem)
                .Append(videoItem); // Replace all video items with new extracted video item 
            
            yield return post with { MediaItems = mediaItems };
        }
    }
}