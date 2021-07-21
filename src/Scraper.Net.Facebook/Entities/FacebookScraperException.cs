using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook
{
    public class FacebookScraperException : Exception
    {
        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("message")]
        public string OriginalMessage { get; init; }

        [JsonPropertyName("stack_trace")]
        public string OriginalStackTrace { get; init; }

        public override string StackTrace => $"{OriginalStackTrace}\n{new StackTrace(this)}";

        public override string Message => $"{Type}: {OriginalMessage}";
    }
}