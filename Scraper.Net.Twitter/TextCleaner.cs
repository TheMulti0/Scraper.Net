using System.Text.RegularExpressions;

namespace Scraper.Net.Twitter
{
    internal class TextCleaner
    {
        private const string ShortenedUrlPattern = @"https://t.co/\S+";
        private const string PicUrlPattern = @"(https://)?pic.twitter.com/\S+";
        private static readonly string MediaUrlPattern = $@"(({TwitterConstants.TwitterBaseUrl}|{TwitterConstants.TwitterBaseUrlWww})/.+/status/\d+/(photo|video)/\d)";
        
        private readonly UrlExpander _urlExpander;

        public TextCleaner()
        {
            _urlExpander = new UrlExpander();
        }

        public string CleanText(string text)
        {
            string withExpandedUrls = Regex.Replace(
                text,
                ShortenedUrlPattern,
                match => _urlExpander.ExpandAsync(match.Groups[0].Value).Result);
            
            return withExpandedUrls.Replace(
                new[]
                {
                    PicUrlPattern,
                    MediaUrlPattern
                },
                string.Empty);
        }
    }
}