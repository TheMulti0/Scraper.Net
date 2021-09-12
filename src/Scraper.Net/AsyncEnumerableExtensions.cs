using System;
using System.Collections.Generic;
using System.Linq;

namespace Scraper.Net
{
    public static class AsyncEnumerableExtensions
    {
        public static IAsyncEnumerable<TSource> Catch<TSource, TException>(
            this IAsyncEnumerable<TSource> enumerable,
            Action<TException> handler)
            where TException : Exception
        {
            IAsyncEnumerable<TSource> Handler(TException e)
            {
                handler(e);
                return null;
            }

            Func<TException,IAsyncEnumerable<TSource>> func = Handler;
            
            return enumerable.Catch(func);
        }
    }
}