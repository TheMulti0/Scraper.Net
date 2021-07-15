using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scraper.Net.Abstractions;
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

        public async Task<IEnumerable<Post>> GetPostsAsync(User user)
        {
            IEnumerable<ITweet> tweets = await _tweetScraper.GetTweetsAsync(user.UserId);

            return tweets.Select(ToPost(user));
        }

        private Func<ITweet, Post> ToPost(User user)
        {
            return tweet =>
            {
                string text = tweet.IsRetweet 
                    ? tweet.RetweetedTweet.Text 
                    : tweet.FullText;
                
                return new Post
                {
                    Content = _textCleaner.CleanText(text),
                    Author = user,
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