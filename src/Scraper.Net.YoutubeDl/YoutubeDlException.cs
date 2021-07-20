using System;

namespace Scraper.Net.YoutubeDl
{
    public class YoutubeDlException : Exception
    {
        public YoutubeDlException(string message) : base(message)
        {
        }
    }
}