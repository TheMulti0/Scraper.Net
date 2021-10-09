using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace Scraper.Net.Youtube
{
    public class YoutubeScraper : IPlatformScraper
    {
        private readonly AsyncLazy<ChannelScraper> _channelScraper;
        private readonly AsyncLazy<VideosScraper> _videosScraper;

        public YoutubeScraper(
            YoutubeConfig config,
            ILoggerFactory loggerFactory)
        {
            var youtubeService =
                new AsyncLazy<YouTubeService>(
                    () => new YouTubeService(
                        new BaseClientService.Initializer
                        {
                            ApiKey = config.ApiKey,
                            ApplicationName = config.AppName
                        }));

            _channelScraper = new AsyncLazy<ChannelScraper>(async () => new ChannelScraper(await youtubeService, loggerFactory.CreateLogger<ChannelScraper>()));
            _videosScraper = new AsyncLazy<VideosScraper>(async () => new VideosScraper(await youtubeService));
        }

        public async Task<Author> GetAuthorAsync(
            string id,
            CancellationToken ct = default)
        {
            Channel channel = await GetChannel(id, ct);
            ChannelSnippet snippet = channel.Snippet;

            return new Author
            {
                Id = channel.Id,
                DisplayName = snippet.Title,
                Description = snippet.Description,
                ProfilePictureUrl = snippet.Thumbnails.High.Url
            };
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            Channel channel = await GetChannel(id, ct);
            
            await foreach (Video video in GetFullVideos(channel, ct))
            {
                yield return ToPost(video, id, channel);
            }
        }

        private static Post ToPost(Video video, string id, Channel channel)
        {
            VideoSnippet snippet = video.Snippet;

            var channelUrl = $"{YoutubeConstants.BaseUrl}/c/{channel.Snippet.CustomUrl}";
            var videoUrl = $"{YoutubeConstants.BaseUrl}/watch?v={video.Id}";

            VideoItem videoItem = GetVideoItem(video, videoUrl);

            return new Post
            {
                Author = new PostAuthor
                {
                    Id = id,
                    DisplayName = channel.Snippet.Title,
                    Url = channelUrl
                },
                Content = $"{snippet.Title}\n \n{snippet.Description}",
                Url = videoUrl,
                CreationDate = snippet.PublishedAt,
                IsLivestream = snippet.LiveBroadcastContent == "live",
                MediaItems = new[]
                {
                    videoItem
                }
            };
        }

        private async Task<Channel> GetChannel(string id, CancellationToken ct)
        {
            ChannelScraper channelScraper = await _channelScraper;
            
            return await channelScraper.GetChannelFromId(id, ct);
        }

        private async IAsyncEnumerable<Video> GetFullVideos(
            Channel channel,
            [EnumeratorCancellation] CancellationToken ct)
        {
            VideosScraper videosScraper = await _videosScraper;
            var videoIds = await GetVideoIds(videosScraper, channel.Id, ct);

            // Get videos by id works with up to 50 video ids at a time
            foreach (IEnumerable<string> videoIdsBatch in videoIds.Batch(50))
            {
                IEnumerable<Video> fullVideosBatch = await videosScraper.GetVideosById(
                    videoIdsBatch.ToArray(),
                    ct);
                
                foreach (Video video in fullVideosBatch)
                {
                    yield return video;
                }
            }
        }

        private static async Task<IEnumerable<string>> GetVideoIds(
            VideosScraper videosScraper,
            string channelId,
            CancellationToken ct)
        {
            IEnumerable<SearchResult> searchResults = await videosScraper.SearchVideosByChannelId(channelId, ct);

            return searchResults
                .Select(result => result.Id.VideoId);
        }

        private static VideoItem GetVideoItem(Video video, string url)
        {
            Thumbnail thumbnail = video.Snippet.Thumbnails.Maxres ??
                                  video.Snippet.Thumbnails.High ??
                                  video.Snippet.Thumbnails.Medium ??
                                  video.Snippet.Thumbnails.Standard ??
                                  video.Snippet.Thumbnails.Default__;

            var duration = XmlConvert.ToTimeSpan(video.ContentDetails.Duration);

            return new VideoItem(
                url,
                UrlType.WebpageUrl,
                thumbnail?.Url,
                duration,
                (int?)thumbnail?.Width,
                (int?)thumbnail?.Height);
        }
    }
}