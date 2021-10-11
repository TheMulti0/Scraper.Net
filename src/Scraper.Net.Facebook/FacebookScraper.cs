using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Scraper.Net.Facebook
{
    /// <summary>
    /// <see cref="IPlatformScraper"/> for providing Facebook posts.
    /// The scraping engine is powered by the python library facebook-scraper
    /// <see href="https://github.com/kevinzg/facebook-scraper"/>
    /// </summary>
    public class FacebookScraper : IPlatformScraper
    {
        private const string SharePrefixPattern = @"‏{0}‏\n‏\d{1,2}‏\s[\w\u0590-\u05FF]+\s·\n";
        private const string PostHyperlinkPattern = @"\/story\.php\?story_fbid=(?<postId>\d+)&id=\d+";

        private static readonly Regex PostHyperlinkRegex = new Regex(PostHyperlinkPattern);
        
        private readonly PostsScraper _postsScraper;
        private readonly PageInfoScraper _pageInfoScraper;

        public FacebookScraper(
            FacebookConfig config,
            ILoggerFactory loggerFactory)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            if (config.MaxPageCount < 1)
            {
                throw new ArgumentException(nameof(config.MaxPageCount));
            }

            var executor = new ScriptExecutor(loggerFactory.CreateLogger<ScriptExecutor>());
            
            _postsScraper = new PostsScraper(executor, config);
            _pageInfoScraper = new PageInfoScraper(executor, config);
        }

        public async Task<Net.Author> GetAuthorAsync(
            string id, 
            CancellationToken ct = default)
        {
            // TODO take advantage of proxy support
            PageInfo pageInfo = await _pageInfoScraper.GetPageInfoAsync(id, null, ct);

            return new Net.Author
            {
                Id = pageInfo.Url.Split('/')[1],
                DisplayName = pageInfo.Name,
                Description = pageInfo.About,
                ProfilePictureUrl = pageInfo.Image
            };
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            IAsyncEnumerable<FacebookPost> posts = _postsScraper.GetFacebookPostsAsync(id, null, ct);
            
            await foreach (FacebookPost facebookPost in posts.WithCancellation(ct))
            {
                yield return ToPost(facebookPost, id);
            }
        }

        private static Post ToPost(FacebookPost post, string id)
        {
            return new Post
            {
                Content = CleanText(post),
                Hyperlinks = GetHyperlinks(post),
                Author = new PostAuthor
                {
                    Id = id,
                    DisplayName = post.Author?.UserName,
                    Url = post.Author?.Url
                },
                OriginalAuthor = GetOriginalAuthor(post),
                CreationDate = post.CreationDate,
                Url = post.Url,
                MediaItems = GetMediaItems(post),
                Type = post.SharedPost == null ? PostType.Post : PostType.Repost,
                IsLivestream = post.IsLive
            };
        }

        private static IEnumerable<Hyperlink> GetHyperlinks(FacebookPost post)
        {
            bool FilterLink(Link link)
            {
                if (link.Text == null)
                {
                    return false;
                }

                Group group = PostHyperlinkRegex
                    .Match(link.Url)
                    .Groups["postId"];

                return group.Value != post.Id;
            }

            Hyperlink ToHyperlink(Link link) => new()
            {
                Text = link.Text,
                Url = $"{FacebookConstants.FacebookBaseUrl}{link.Url}"
            };

            return post.Links
                .Where(FilterLink)
                .Select(ToHyperlink);
        }

        private static PostAuthor GetOriginalAuthor(FacebookPost post)
        {
            if (post.SharedPost?.Author != null)
            {
                return new PostAuthor
                {
                    Id = post.SharedPost.Author.Id,
                    DisplayName = post.SharedPost.Author.UserName,
                    Url = post.SharedPost.Author.Url
                };
            }
            
            return null;
        }

        private static string CleanText(FacebookPost post)
        {
            if (post.SharedPost?.Text == null)
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
            FacebookSharedPost sharedPost = post.SharedPost;
            string author = sharedPost.Author.UserName;
            string text = sharedPost.Text;

            if (author == null)
            {
                return text;
            }
            
            var regex = new Regex(SharePrefixPattern.Replace("{0}", author));
            return regex.Replace(text, string.Empty);
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
                UrlType.DirectUrl,
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