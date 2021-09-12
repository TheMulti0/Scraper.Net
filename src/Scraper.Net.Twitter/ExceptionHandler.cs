using System;
using System.Collections.Generic;
using System.Net;
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
        
        public static IAsyncEnumerable<T> HandleExceptionAsync<T>(string id, Func<IAsyncEnumerable<T>> func)
        {
            void HandleException(TwitterException e)
            {
                switch ((HttpStatusCode?)e.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new IdNotFoundException(id, e);

                    case HttpStatusCode.TooManyRequests:
                        throw new RateLimitedException(e);

                    default:
                        throw e;
                }
            }

            return func().Catch<T, TwitterException>(HandleException);
        }
    }
}