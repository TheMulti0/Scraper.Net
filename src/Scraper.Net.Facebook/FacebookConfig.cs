using System;

namespace Scraper.Net.Facebook
{
    public class FacebookConfig
    {
        public string PythonPath { get; init; } = "python3";
        
        public int MaxPageCount { get; set; } = 1;

        public int PostsPerPage { get; set; } = 5;
        
        public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);

        public string[] CookiesFileNames { get; set; } = Array.Empty<string>();
    }
}