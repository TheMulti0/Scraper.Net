using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Scraper.Net.Feed
{
    internal static class ExceptionHandler
    {
        public static T Do<T>(string id, Func<T> func)
        {
            try
            {
                return func();
            }
            catch (FileNotFoundException e)
            {
                throw new IdNotFoundException(id, e);
            }
            catch (HttpRequestException e)
            {
                switch (e.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new IdNotFoundException(id, e);
                    
                    case null when e.Message.StartsWith("No such host is known") ||
                                   e.Message.StartsWith("Resource temporarily unavailable"):
                        throw new IdNotFoundException(id, e);
                    
                    case HttpStatusCode.TooManyRequests:
                        throw new RateLimitedException(e);
                    
                    default:
                        throw;
                }
            }
        }
    }
}