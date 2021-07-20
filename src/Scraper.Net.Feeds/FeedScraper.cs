using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Scraper.Net.Feeds
{
    public class FeedScraper : IPlatformScraper
    {
        private const string ImageSrcPattern = "<img.+?src=[\"'](.+?)[\"'].*?>";
        private static readonly Regex ImageSrcRegex = new(ImageSrcPattern);

        public Task<Author> GetAuthorAsync(
            string id,
            CancellationToken ct = default)
        {
            return Task.FromResult(GetAuthor(id));
        }

        private static Author GetAuthor(string id)
        {
            SyndicationFeed feed = GetFeed(id);

            return new Author
            {
                Id = id,
                DisplayName = feed.Title?.Text,
                Description = feed.Description?.Text,
                ProfilePictureUrl = feed.ImageUrl?.ToString()
            };
        }

        public IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            CancellationToken ct = default)
        {
            SyndicationFeed feed = GetFeed(id);

            return feed.Items.Select(item => ToPost(item, id)).ToAsyncEnumerable();
        }

        private static SyndicationFeed GetFeed(string id)
        {
            using XmlReader reader = ExceptionHandler.Do(id, () => XmlReader.Create(id));
            
            return SyndicationFeed.Load(reader);
        }

        private static Post ToPost(SyndicationItem item, string url)
        {
            return new()
            {
                Content = item.Title.Text + "\n \n" + item.Summary.Text,
                AuthorId = url,
                CreationDate = item.PublishDate.DateTime,
                Url = item.Links.FirstOrDefault()?.Uri.ToString(),
                MediaItems = GetMediaItems(item),
                Type = PostType.Post,
                IsLivestream = false
            };
        }

        private static IEnumerable<IMediaItem> GetMediaItems(SyndicationItem item)
        {
            Group urlGroup = ImageSrcRegex.Match(item.Summary.Text).Groups[1];
            string url = urlGroup.Value;
            
            if (!urlGroup.Success || string.IsNullOrWhiteSpace(url))
            {
                yield break;
            }
            
            yield return new PhotoItem(url);
        }
    }
}