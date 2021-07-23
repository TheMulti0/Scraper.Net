using System;

namespace Scraper.Net
{
    /// <summary>
    /// Thrown when a platform scraper is rate-limited
    /// </summary>
    public class RateLimitedException : Exception
    {
        public RateLimitedException()
        {
        }
        
        public RateLimitedException(Exception e) : this(string.Empty, e)
        {
        }
        
        public RateLimitedException(string message, Exception e) : base(message, e)
        {
        }
    }
}