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
        public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);

        [JsonPropertyName("timeout")]
        public int TimeoutSeconds => (int) Timeout.TotalSeconds;

        [JsonPropertyName("cookies_filename")]
        public string CookiesFileName { get; init; }
    }
}