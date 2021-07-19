using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;

namespace Scraper.Net.YoutubeDl
{
    public class YoutubeDlPostProcessor : IPostProcessor
    {
        private readonly bool _keepReceivedPost;
        private readonly YoutubeDL _youtubeDl;
        private readonly OptionSet _overrideOptions;

        public YoutubeDlPostProcessor(
            bool keepReceivedPost,
            YoutubeDlConfig config)
        {
            _keepReceivedPost = keepReceivedPost;
            
            _youtubeDl = new YoutubeDL(config.DegreeOfConcurrency)
            {
                YoutubeDLPath = config.YoutubeDlPath
            };

            _overrideOptions = config.OverrideOptions;
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
            
            VideoItem videoItem = await ExtractVideoItem(post.Url, ct);

            IEnumerable<IMediaItem> postMediaItems = post.MediaItems ?? Enumerable.Empty<IMediaItem>();
            
            IEnumerable<IMediaItem> mediaItems = postMediaItems
                .Where(item => item is not VideoItem)
                .Append(videoItem); // Append extracted video to media items
            
            yield return post with { MediaItems = mediaItems };
        }

        private async Task<VideoItem> ExtractVideoItem(string postUrl, CancellationToken ct)
        {
            VideoData data = await GetVideoData(postUrl, ct);

            FormatData highestFormat = GetHighestQualityFormat(data);
            ThumbnailData highestThumbnail = GetHighestQualityThumbnail(data);

            return new VideoItem(
                highestFormat.Url ?? data.Url,
                highestThumbnail.Url,
                data.Duration,
                highestFormat.Width ?? highestThumbnail.Width,
                highestFormat.Height ?? highestThumbnail.Height);
        }

        private static FormatData GetHighestQualityFormat(VideoData data)
        {
            const string none = "none";

            return data.Formats.LastOrDefault(
                format => format.AudioCodec != none && format.VideoCodec != none);
        }

        private static ThumbnailData GetHighestQualityThumbnail(VideoData data) 
            => data.Thumbnails.LastOrDefault();

        private async Task<VideoData> GetVideoData(string url, CancellationToken ct)
        {
            RunResult<VideoData> result = await _youtubeDl.RunVideoDataFetch(url, ct, overrideOptions: _overrideOptions);

            if (result.Success)
            {
                return result.Data;
            }
            
            string message = string.Join('\n', result.ErrorOutput);
            throw new Exception(message);
        }
    }
}