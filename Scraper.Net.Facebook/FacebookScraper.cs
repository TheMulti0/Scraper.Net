using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Scraper.Net.Facebook
{
    public class FacebookScraper : IPlatformScraper
    {
        private const string SharePrefixPattern = @"‏{0}‏\n‏\d{1,2}‏\s[\w\u0590-\u05FF]+\s·\n";
        private readonly FacebookPostsScraper _scraper;

        public FacebookScraper(FacebookScraperConfig config)
        {
            _scraper = new FacebookPostsScraper(config);
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            IEnumerable<FacebookPost> posts = await _scraper.GetPostsAsync(id, ct);

            foreach (Post post in posts.Select(ToPost(id)))
            {
                yield return post;
            }
        }

        private Func<FacebookPost, Post> ToPost(string id)
        {
            return post => new Post
            {
                Content = CleanText(post),
                AuthorId = id,
                CreationDate = post.CreationDate,
                Url = post.Url,
                MediaItems = GetMediaItems(post),
                Type = post.SharedPost == null ? PostType.Post : PostType.Repost,
                IsLivestream = post.IsLive
            };
        }

        private string CleanText(FacebookPost post)
        {
            if (post.SharedPost == null)
            {
                return post.EntireText;
            }
            
            string sharedPostText = GetSharedPostText(post);

            return string.IsNullOrEmpty(post.PostTextOnly) 
                ? sharedPostText 
                : $"{post.PostTextOnly}\n---\n{sharedPostText}";
        }

        private static string GetSharedPostText(FacebookPost post)
        {
            var regex = new Regex(SharePrefixPattern.Replace("{0}", post.SharedPost.Author.UserName));

            return regex.Replace(post.SharedPost.Text, string.Empty);
        }

        private static IEnumerable<IMediaItem> GetMediaItems(FacebookPost post)
        {
            IEnumerable<PhotoItem> photos = GetPhotoItems(post);

            if (post.Video == null)
            {
                return photos;
            }

            if (post.IsLive)
            {
                return new List<IMediaItem>();
            }
            
            var video = new VideoItem(
                post.Video.Url,
                post.Video.ThumbnailUrl,
                post.Video.Duration,
                post.Video.Width,
                post.Video.Height);

            return photos
                .Concat(new IMediaItem[] { video });
        }

        private static IEnumerable<PhotoItem> GetPhotoItems(FacebookPost post)
        {
            return post.Images?.Select(img => new PhotoItem(img.Url)) ??
                   Enumerable.Empty<PhotoItem>();
        }
    }
}