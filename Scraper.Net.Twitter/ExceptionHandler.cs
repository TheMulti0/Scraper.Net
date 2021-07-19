using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tweetinvi.Exceptions;

namespace Scraper.Net.Twitter
{
    internal static class ExceptionHandler
    {
        public static async Task<T> HandleExceptionAsync<T>(string id, Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch (TwitterException e)
            {
                switch (e.StatusCode)
                {
                    case 404:
                        throw new IdNotFoundException(id, e);
                    case 429:
                        throw new RateLimitedException(e);
                    default:
                        throw;
                }
            }
        }
        
        public static async IAsyncEnumerable<T> HandleExceptionAsync<T>(string id, Func<IAsyncEnumerable<T>> func)
        {
            IAsyncEnumerator<T> enumerator = func().GetAsyncEnumerator();
            
            while (true)
            {
                T current;
                
                try
                {
                    if (!await enumerator.MoveNextAsync())
                    {
                        break;
                    }
                    current = enumerator.Current;
                }
                catch (TwitterException e)
                {
                    switch (e.StatusCode)
                    {
                        case 404:
                            throw new IdNotFoundException(id, e);
                        case 429:
                            throw new RateLimitedException(e);
                        default:
                            throw;
                    }
                }
                
                // the yield statement is outside the try catch block
                yield return current;
            }
        }
    }
}