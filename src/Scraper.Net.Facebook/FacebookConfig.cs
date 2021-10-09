namespace Scraper.Net.Facebook
{
    public class FacebookConfig
    {
        public string PythonPath { get; init; } = "python3";
        
        public int MaxPageCount { get; set; } = 1;

        public int PostsPerPage { get; set; } = 5;

        public string CookiesFileName { get; set; }
    }
}