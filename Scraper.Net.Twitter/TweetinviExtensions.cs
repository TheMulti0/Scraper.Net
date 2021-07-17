using System.Collections.Generic;
using Tweetinvi.Iterators;

namespace Scraper.Net.Twitter
{
    internal static class TweetinviExtensions
    {
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this ITwitterIterator<T> iterator)
        {
            while (!iterator.Completed)
            {
                foreach (var item in await iterator.NextPageAsync())
                {
                    yield return item;
                }
            }
        }

        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T, U>(this ITwitterIterator<T, U> iterator)
        {
            while (!iterator.Completed)
            {
                foreach (var item in await iterator.NextPageAsync())
                {
                    yield return item;
                }
            }
        }
    }
}