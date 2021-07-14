using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Scraper.Net.Abstractions;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;
using Tweetinvi.Models.Entities.ExtendedEntities;
using Tweetinvi.Parameters;

namespace Scraper.Net.Twitter
{
    public class TwitterUpdatesProvider : IPlatformScraper
    {
        internal ITwitterClient TwitterClient { get; }
        
        private readonly UrlExpander _urlExpander;

        public TwitterUpdatesProvider(
            TwitterScraperConfig config)
        {
            TwitterClient = new TwitterClient(
                config.ConsumerKey,
                config.ConsumerSecret);

            TwitterClient.Auth.InitializeClientBearerTokenAsync().Wait();

            _urlExpander = new UrlExpander();
        }

        public Task<IEnumerable<Post>> GetPostsAsync(User user)
        {
            return GetUpdatesAsync(user, false);
        }
        
        public async Task<IEnumerable<Post>> GetUpdatesAsync(
            User user, bool includeAll)
        {
            var parameters = new GetUserTimelineParameters(user.UserId)
            {
                PageSize = 10,
                TweetMode = TweetMode.Extended
            };
            IEnumerable<ITweet> tweets = await TwitterClient.Timelines.GetUserTimelineAsync(parameters);

            if (!includeAll)
            {
                tweets = tweets.Where(IsTweetPublishable(user.UserId));
            }
                
            return tweets.Select(ToPost(user));
        }

        private static Func<ITweet, bool> IsTweetPublishable(string userId)
        {
            return tweet => tweet.InReplyToStatusId == null ||
                            tweet.InReplyToScreenName.Equals(userId, StringComparison.CurrentCultureIgnoreCase); 
        }

        private Func<ITweet, Post> ToPost(User user)
        {
            return tweet => new Post
            {
                Content = CleanText(tweet.IsRetweet 
                    ? tweet.RetweetedTweet.Text 
                    : tweet.FullText),
                Author = user,
                CreationDate = tweet.CreatedAt.DateTime,
                Url = tweet.Url,
                MediaItems = GetMediaItems(tweet),
                Type = GetPostType(tweet)
            };
        }

        internal string CleanText(string text)
        {
            string withExpandedUrls = Regex.Replace(
                text,
                @"https://t.co/\S+",
                match => _urlExpander.ExpandAsync(match.Groups[0].Value).Result);

            return withExpandedUrls.Replace(
                new[]
                {
                    @"(https://)?pic.twitter.com/\S+",
                    $@"(({TwitterConstants.TwitterBaseUrl}|{TwitterConstants.TwitterBaseUrlWww})/.+/status/\d+/(photo|video)/\d)"
                },
                string.Empty);
        }
        
        internal static IEnumerable<IMediaItem> GetMediaItems(ITweet tweet)
        {
            List<IMediaEntity> medias = tweet.ExtendedTweet?.ExtendedEntities?.Medias 
                                        ?? tweet.Media 
                                        ?? new List<IMediaEntity>();

            foreach (IMediaEntity media in medias)
            {
                string url = media.MediaURLHttps ?? media.MediaURL;

                if (media.MediaType == "photo")
                {
                    yield return new PhotoItem(url);
                }
                else
                {
                    IMediaItem video = GetVideoItem(media, url);
                    
                    if (video != null)
                    {
                        yield return video;
                    }
                }
            }
        }

        private static IMediaItem GetVideoItem(IMediaEntity media, string thumbnailUrl)
        {
            IVideoInformationEntity videoInfo = media.VideoDetails;
            IVideoEntityVariant[] variants = videoInfo.Variants;

            IVideoEntityVariant bestVideo = variants.OrderByDescending(variant => variant.Bitrate)
                .FirstOrDefault();

            Dictionary<string, IMediaEntitySize> sizes = media.Sizes;
            IMediaEntitySize size = sizes.GetValueOrDefault("large") 
                                    ?? sizes.GetValueOrDefault("medium") 
                                    ?? sizes.GetValueOrDefault("small");

            if (bestVideo != null)
            {
                return new VideoItem(
                        bestVideo.URL,
                        thumbnailUrl,
                        TimeSpan.FromMilliseconds(videoInfo.DurationInMilliseconds),
                        size?.Width,
                        size?.Height);
            }

            return null;
        }

        private static PostType GetPostType(ITweet tweet)
        {
            if (tweet.InReplyToScreenName != null)
            {
                return PostType.Reply;
            }

            return tweet.IsRetweet 
                ? PostType.Repost 
                : PostType.Post;
        }
    }
}