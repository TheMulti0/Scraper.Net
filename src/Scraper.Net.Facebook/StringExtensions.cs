namespace Scraper.Net.Facebook
{
    internal static class StringExtensions
    {
        public static string EscapeQuotes(this string source)
        {
            return source.Replace("\"", "\\\"");
        }
    }
}