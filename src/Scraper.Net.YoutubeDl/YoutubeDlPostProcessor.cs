﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
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
        private readonly YoutubeDL _youtubeDl;
        private readonly OptionSet _overrideOptions;

        public YoutubeDlPostProcessor(
            YoutubeDlConfig config)
        {
            _keepReceivedPost = config.KeepReceivedPost;
            
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
            
            if (!post.MediaItems.Any(item => item is VideoItem))
            {
                yield break;
            }
            
            VideoItem videoItem = await ExtractVideoItem(post.Url, ct);

            IEnumerable<IMediaItem> mediaItems = post.MediaItems
                .Where(item => item is not VideoItem)
                .Append(videoItem); // Replace all video items with new extracted video item 
            
            yield return post with { MediaItems = mediaItems };
        }

        private async Task<VideoItem> ExtractVideoItem(string postUrl, CancellationToken ct)
        {
            VideoData data = await ExceptionHandler.Do(() => GetVideoData(postUrl, ct));

            FormatData highestFormat = GetHighestQualityFormat(data);
            ThumbnailData highestThumbnail = GetHighestQualityThumbnail(data);
            
            return new VideoItem(
                highestFormat.Url ?? data.Url,
                UrlType.DirectUrl,
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
            throw new YoutubeDlException(message);
        }
    }
}