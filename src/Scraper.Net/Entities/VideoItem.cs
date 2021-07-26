﻿using System;
using System.Text.Json.Serialization;

namespace Scraper.Net
{
    public record VideoItem : IMediaItem
    {
        public string Url { get; }
        public UrlType UrlType { get; }
        public string ThumbnailUrl { get; }
        [JsonConverter(typeof(NullableTimeSpanConverter))]
        public TimeSpan? Duration { get; }
        public int? Width { get; }
        public int? Height { get; }

        [JsonConstructor]
        public VideoItem(
            string url,
            UrlType urlType,
            string thumbnailUrl,
            TimeSpan? duration = null,
            int? width = null,
            int? height = null)
        {
            Url = url;
            UrlType = urlType;
            ThumbnailUrl = thumbnailUrl;
            Duration = duration;
            Width = width;
            Height = height;
        }
        
        public VideoItem(
            string url,
            UrlType urlType,
            string thumbnailUrl,
            double? durationSeconds = null,
            int? width = null,
            int? height = null)
        {
            Url = url;
            UrlType = urlType;
            ThumbnailUrl = thumbnailUrl;
            if (durationSeconds != null)
            {
                Duration = TimeSpan.FromSeconds((double) durationSeconds);
            }
            Width = width;
            Height = height;
        }
    }
}