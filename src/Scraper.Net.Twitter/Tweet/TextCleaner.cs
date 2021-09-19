using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Twitter
{
    internal class TextCleaner
    {
        private const string ShortenedUrlPattern = @"https://t.co/\S+";
        private const string PicUrlPattern = @"(https://)?pic.twitter.com/\S+";
        private static readonly string MediaUrlPattern = $@"(({TwitterConstants.TwitterBaseUrl}|{TwitterConstants.TwitterBaseUrlWww})/.+/status/\d+/(photo|video)/\d)";
        private static readonly Regex ShortenedUrlRegex = new(ShortenedUrlPattern);
        
        private readonly UrlExpander _urlExpander;

        public TextCleaner()
        {
            _urlExpander = new UrlExpander();
        }

        public async Task<string> CleanTextAsync(string text, CancellationToken ct = default)
        {
            string withExpandedUrls = await ShortenedUrlRegex.ReplaceAsync(
                text,
                match => _urlExpander.ExpandAsync(match.Groups[0].Value),
                ct);
            
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