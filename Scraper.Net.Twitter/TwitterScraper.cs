using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace Scraper.Net.Twitter
{
    public class TwitterScraper : IPlatformScraper
    {
        private readonly TweetScraper _tweetScraper;
        private readonly TextCleaner _textCleaner;
        private readonly MediaItemsExtractor _mediaItemsExtractor;

        public TwitterScraper(
            TwitterScraperConfig config)
        {
            _tweetScraper = new TweetScraper(config);
            _textCleaner = new TextCleaner();
            _mediaItemsExtractor = new MediaItemsExtractor();
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(
            string id, 
            CancellationToken ct = default)
        {
            IEnumerable<ITweet> tweets = await _tweetScraper.GetTweetsAsync(id);

            return await tweets.ToAsyncEnumerable().SelectAwaitWithCancellation(ToPost(id)).ToListAsync(ct);
        }

        private Func<ITweet, CancellationToken, ValueTask<Post>> ToPost(string id)
        {
            return async (tweet, ct) =>
            {
                string text = tweet.IsRetweet 
                    ? tweet.RetweetedTweet.Text 
                    : tweet.FullText;
                
                return new Post
                {
                    Content = await _textCleaner.CleanTextAsync(text, ct),
                    AuthorId = id,
                    CreationDate = tweet.CreatedAt.DateTime,
                    Url = tweet.Url,
                    MediaItems = _mediaItemsExtractor.ExtractMediaItems(tweet),
                    Type = GetPostType(tweet)
                };
            };
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