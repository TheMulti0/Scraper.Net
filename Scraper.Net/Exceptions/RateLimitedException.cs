using System;

namespace Scraper.Net
{
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