using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Models;

namespace Scraper.Net.Twitter
{
    /// <summary>
    /// <see cref="IPlatformScraper"/> for providing Twitter posts.
    /// The scraping engine is powered by Tweetinvi
    /// </summary>
    public class TwitterScraper : IPlatformScraper
    {
        private readonly AsyncLazy<TweetScraper> _tweetScraper;
        private readonly AsyncLazy<UserScraper> _userScraper;
        private readonly TextCleaner _textCleaner;
        private readonly MediaItemsExtractor _mediaItemsExtractor;

        public TwitterScraper(TwitterConfig config)
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
            IUser user = await GetUserAsync(id, userScraper);

            return new Author
            {
                Id = user.ScreenName,
                DisplayName = user.Name,
                Description = user.Description,
                ProfilePictureUrl = user.ProfileImageUrl
            };
        }

        private static Task<IUser> GetUserAsync(string id, UserScraper userScraper)
        {
            return ExceptionHandler.HandleExceptionAsync(id, () => userScraper.GetUserAsync(id));
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(
            string id, 
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            TweetScraper tweetScraper = await _tweetScraper;

            IAsyncEnumerable<ITweet> tweets = GetTweetsAsync(id, tweetScraper);
            
            IAsyncEnumerable<Post> posts = tweets.SelectAwaitWithCancellation(ToPost(id));
            
            await foreach (Post post in posts.WithCancellation(ct))
            {
                yield return post;
            }
        }

        private static IAsyncEnumerable<ITweet> GetTweetsAsync(string id, TweetScraper tweetScraper)
        {
            return ExceptionHandler.HandleExceptionAsync(id, () => tweetScraper.GetTweetsAsync(id));
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