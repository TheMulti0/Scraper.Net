namespace Scraper.Net.Facebook
{
    public class FacebookConfig
    {
        public string PythonPath { get; init; } = "python3";
        
        public int PageCount { get; set; } = 1;

        public string[] Proxies { get; set; } = new string[0];

        public string CookiesFileName { get; set; }
    }
}