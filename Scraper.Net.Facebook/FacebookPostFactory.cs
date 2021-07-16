using System.Collections.Generic;
using System.Linq;
using Scraper.Net.Facebook.Entities;
using Scraper.Net.Facebook.Entities.Raw;

namespace Scraper.Net.Facebook
{
    internal static class FacebookPostFactory
    {
        private const string LinkRegex = "\n(?<link>[A-Z].+)";

        public static FacebookPost ToPost(this RawFacebookPost raw)
        {
            return new()
            {
                Id = raw.PostId,
                EntireText = raw.GetCleanText(),
                PostTextOnly = raw.PostText,
                CreationDate = raw.Time,
                Url = raw.PostUrl,
                Author = new Author
                {
                    Id = raw.UserId,
                    Url = raw.UserUrl,
                    UserName = raw.UserName
                },
                IsLive = raw.IsLive,
                Link = raw.Link,
                Images = raw.GetImages().ToArray(),
                Video = raw.GetVideo(),
                Stats = new Stats
                {
                    Comments = raw.Comments,
                    Shares = raw.Shares,
                    Likes = raw.Likes
                },
                SharedPost = raw.GetSharedPost(),
                Comments = raw.CommentsFull
            };
        }
        
        private static string GetCleanText(this RawFacebookPost raw)
        {
            if (raw.Link != null)
            {
                return raw.Text.Replace(
                    new[]
                    {
                        LinkRegex
                    },
                    raw.Link);
            }

            return raw.Text;
        }

        private static IEnumerable<Image> GetImages(this RawFacebookPost raw)
        {
            return raw.ImageIds.Select((id, index) => new Image
            {
                Id = id,
                Url = raw.Images.ElementAt(index),
                LowQualityUrl = raw.ImagesLowQuality.ElementAtOrDefault(index),
                Description = raw.ImagesDescription.ElementAtOrDefault(index)
            });
        }

        private static Video GetVideo(this RawFacebookPost raw)
        {
            if (raw.VideoId == null)
            {
                return null;
            }   
            
            return new Video
            {
                Id = raw.VideoId,
                Url = raw.VideoUrl,
                Width = raw.VideoWidth,
                Height = raw.VideoHeight,
                Quality = raw.VideoQuality,
                ThumbnailUrl = raw.VideoThumbnail,
                SizeMb = raw.VideoSizeMb,
                Watches = raw.VideoWatches
            };
        }

        private static FacebookSharedPost GetSharedPost(this RawFacebookPost raw)
        {
            if (raw.SharedPostId == null)
            {
                return null;
            }
            
            return new FacebookSharedPost
            {
                Id = raw.SharedPostId,
                Url = raw.SharedPostUrl,
                Text = raw.SharedText,
                CreationDate = raw.SharedTime,
                Author = new Author
                {
                    Id = raw.SharedUserId,
                    UserName = raw.SharedUserName
                }
            };
        }
    }
}