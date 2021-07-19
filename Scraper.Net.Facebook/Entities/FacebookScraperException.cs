using System;
using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook
{
    public class FacebookScraperException : Exception
    {
        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("message")]
        public string OriginalMessage { get; init; }

        public override string Message => OriginalMessage;
    }
}