using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Tweetinvi.Iterators;

namespace Scraper.Net.Twitter
{
    internal static class TweetinviExtensions
    {
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(
            this ITwitterIterator<T> iterator,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            while (!iterator.Completed)
            {
                foreach (var item in await iterator.NextPageAsync())
                {
                    yield return item;
                }
                ct.ThrowIfCancellationRequested();
            }
        }

        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T, U>(
            this ITwitterIterator<T, U> iterator,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            while (!iterator.Completed)
            {
                foreach (T item in await iterator.NextPageAsync())
                {
                    yield return item;
                }
                ct.ThrowIfCancellationRequested();
            }
        }
    }
}