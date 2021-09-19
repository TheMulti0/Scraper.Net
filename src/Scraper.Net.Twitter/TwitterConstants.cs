namespace Scraper.Net.Twitter
{
    public static class TwitterConstants
    {
        public const string PlatformName = "twitter";
        
        public const string TwitterBaseDomain = "twitter.com";

        internal static readonly string TwitterBaseUrl = $"https://{TwitterBaseDomain}";
        internal static readonly string TwitterBaseUrlWww = $"https://www.{TwitterBaseDomain}";
    }
}