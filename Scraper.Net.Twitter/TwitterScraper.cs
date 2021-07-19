using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;

namespace Scraper.Net.Twitter
{
    public class TwitterScraper : IPlatformScraper
    {
        private readonly AsyncLazy<TweetScraper> _tweetScraper;
        private readonly AsyncLazy<UserScraper> _userScraper;
        private readonly TextCleaner _textCleaner;
        private readonly MediaItemsExtractor _mediaItemsExtractor;

        public TwitterScraper(
            TwitterConfig config)
        {
            if (config.MaxPageCount < 1)
            {
                throw new ArgumentException(nameof(config.MaxPageCount));
            }
            if (config.MaxPageSize < 1)
            {
                throw new ArgumentException(nameof(config.MaxPageSize));
            }
            
            var twitterClient = new AsyncLazy<ITwitterClient>(() => TwitterClientFactory.CreateAsync(config));

            _tweetScraper = new AsyncLazy<TweetScraper>(async () => new TweetScraper(await twitterClient, config));
            _userScraper = new AsyncLazy<UserScraper>(async () => new UserScraper(await twitterClient));
            
            _textCleaner = new TextCleaner();
            _mediaItemsExtractor = new MediaItemsExtractor();
        }

        public async Task<Author> GetAuthorAsync(
            string id,
            CancellationToken ct = default)
        {
            UserScraper userScraper = await _userScraper;
            IUser user = await userScraper.GetUserAsync(id);

            return new Author
            {
                Id = user.ScreenName,
                DisplayName = user.Name,
                Description = user.Description,
                ProfilePictureUrl = user.ProfileImageUrl
            };
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(
            string id, 
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            TweetScraper tweetScraper = await _tweetScraper;
            IAsyncEnumerable<ITweet> tweets = tweetScraper.GetTweetsAsync(id);

            IAsyncEnumerable<Post> posts = tweets.SelectAwaitWithCancellation(ToPost(id));
            
            await foreach (Post post in posts.WithCancellation(ct))
            {
                yield return post;
            }
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