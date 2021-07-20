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
                case "LoginRequired":
                    throw new LoginRequiredException($"Login required in order to view {id}", e);
                
                case "ProxyError":
                case "HTTPError" when e.Message.StartsWith("409 Client Error: Conflict for url: http://lumtest.com/myip.json"):
                    throw new InvalidOperationException($"Proxy is invalid, proxy is {proxy}", e);
                case "InvalidCookies":
                    throw new InvalidOperationException("Invalid cookies passed in the cookies file", e);

                default:
                    throw e;
            }
        }
    }
}