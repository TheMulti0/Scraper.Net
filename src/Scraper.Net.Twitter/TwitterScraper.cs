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
            
            IAsyncEnumerable<Post> posts = ToPosts(tweets, id, ct);
            
            await foreach (Post post in posts.WithCancellation(ct))
            {
                yield return post;
            }
        }

        private static IAsyncEnumerable<ITweet> GetTweetsAsync(string id, TweetScraper tweetScraper)
        {
            return ExceptionHandler.HandleExceptionAsync(id, () => tweetScraper.GetTweetsAsync(id));
        }

        private async IAsyncEnumerable<Post> ToPosts(
            IAsyncEnumerable<ITweet> tweets,
            string id,
            [EnumeratorCancellation] CancellationToken ct)
        {
            var enumerator = tweets.GetAsyncEnumerator(ct);

            while (true)
            {
                if (!await enumerator.MoveNextAsync())
                {
                    break;
                }

                var batch = GroupReplies(enumerator, enumerator.Current);

                yield return await batch
                    .SelectAwaitWithCancellation((tweet, c) => ToPost(tweet, id, c))
                    .AggregateAsync(
                        (reply, source) => source with { ReplyPost = reply },
                        ct);
            }
        }

        private static async IAsyncEnumerable<ITweet> GroupReplies(
            IAsyncEnumerator<ITweet> enumerator,
            ITweet current)
        {
            ITweet prev = null;

            do
            {
                yield return current;

                prev = current;
                if (!await enumerator.MoveNextAsync())
                {
                    break;
                }
                current = enumerator.Current;
            }
            while (prev?.InReplyToStatusId == current.Id);
        }

        private async ValueTask<Post> ToPost(ITweet tweet, string id, CancellationToken ct)
        {
            string text = tweet.IsRetweet 
                ? tweet.RetweetedTweet.Text 
                : tweet.FullText;

            PostType postType = GetPostType(tweet, id);
            PostAuthor originalAuthor = GetOriginalAuthor(tweet, postType);

            return new Post
            {
                Content = await _textCleaner.CleanTextAsync(text, ct),
                Author = new PostAuthor
                {
                    Id = id,
                    DisplayName = tweet.CreatedBy.Name,
                    Url = tweet.CreatedBy.Url
                },
                OriginalAuthor = originalAuthor,
                CreationDate = tweet.CreatedAt.DateTime,
                Url = tweet.Url,
                MediaItems = _mediaItemsExtractor.ExtractMediaItems(tweet),
                Type = postType
            };
        }

        private static PostAuthor GetOriginalAuthor(ITweet tweet, PostType postType)
        {
            switch (postType)
            {
                case PostType.Repost:
                    return new PostAuthor
                    {
                        Id = tweet.RetweetedTweet.CreatedBy.ScreenName,
                        DisplayName = tweet.RetweetedTweet.CreatedBy.Name,
                        Url = tweet.RetweetedTweet.CreatedBy.Url
                    };
                
                case PostType.Reply:
                    return new PostAuthor
                    {
                        Id = tweet.InReplyToScreenName,
                        DisplayName = tweet.InReplyToScreenName,
                        Url = $"{TwitterConstants.TwitterBaseUrl}/{tweet.InReplyToScreenName}"
                    };
                
                default:
                    return null;
            }
        }

        private static PostType GetPostType(ITweet tweet, string userId)
        {
            if (tweet.InReplyToStatusId == null)
            {
                return tweet.IsRetweet
                    ? PostType.Repost
                    : PostType.Post;
            }
            return tweet.InReplyToScreenName.Equals(userId, StringComparison.CurrentCultureIgnoreCase) 
                ? PostType.ReplyToSelf 
                : PostType.Reply;
        }
    }
}