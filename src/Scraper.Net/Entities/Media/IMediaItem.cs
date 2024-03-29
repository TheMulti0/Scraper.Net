﻿namespace Scraper.Net
{
    /// <summary>
    /// Represents the base unit of a media item of a post 
    /// </summary>
    [JsonInterfaceConverter(typeof(InterfaceConverter<IMediaItem>))]
    public interface IMediaItem
    {
        public string Url { get; }
    }
}