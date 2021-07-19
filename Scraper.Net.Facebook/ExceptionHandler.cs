using System;

namespace Scraper.Net.Facebook
{
    internal static class ExceptionHandler
    {
        public static void HandleException(string id, string proxy, FacebookScraperException e)
        {
            switch (e.Type)
            {
                case "NotFound":
                case "HTTPError" when e.Message.StartsWith("404"):
                    throw new IdNotFoundException(id, e);
                case "TemporarilyBanned":
                    throw new RateLimitedException($"Temporarily banned, proxy is {proxy}", e);
                
                case "ProxyError":
                    throw new InvalidOperationException($"Proxy is invalid, proxy is {proxy}", e);
                case "InvalidCookies":
                    throw new InvalidOperationException("Invalid cookies passed in the cookies file", e);
                case "LoginRequired":
                    throw new InvalidOperationException($"Login required in order to view {id}", e);

                default:
                    throw e;
            }
        }
    }
}