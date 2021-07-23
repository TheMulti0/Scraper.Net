using System;

namespace Scraper.Net
{
    /// <summary>
    /// Thrown when a platform scraper requires login 
    /// </summary>
    public class LoginRequiredException : Exception
    {
        public LoginRequiredException()
        {
        }
        
        public LoginRequiredException(Exception e) : this(string.Empty, e)
        {
        }
        
        public LoginRequiredException(string message, Exception e) : base(message, e)
        {
        }
    }
}