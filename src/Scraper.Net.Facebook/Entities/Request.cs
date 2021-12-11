using System;
using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook
{
    internal record Request
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; init; }
        
        [JsonPropertyName("proxy")]
        public string Proxy { get; init; }

        [JsonIgnore]
        public TimeSpan Timeout { get; init; }

        [JsonPropertyName("timeout")]
        public int TimeoutSeconds => (int) Timeout.TotalSeconds;

        [JsonPropertyName("cookies_filenames")]
        public string[] CookiesFileNames { get; init; }
    }
}