using System;
using System.Collections.Generic;
using System.Linq;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;
using Tweetinvi.Models.Entities.ExtendedEntities;

namespace Scraper.Net.Twitter
{
    internal class MediaItemsExtractor
    {
        public IEnumerable<IMediaItem> ExtractMediaItems(ITweet tweet)
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
    }
}