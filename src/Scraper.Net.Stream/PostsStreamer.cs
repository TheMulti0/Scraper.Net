﻿using System;

namespace Scraper.Net.Stream
{
    public class PostsStreamer
    {
        private readonly IScraperService _service;
        private readonly PostFilter _filter;

        public PostsStreamer(
            IScraperService service,
            PostFilter filter)
        {
            _service = service;
            _filter = filter;
        }

        public IObservable<Post> Stream(
            string id,
            string platform,
            TimeSpan interval)
        {
            return PollingStreamer.Stream(
                ct => _service.GetPostsAsync(id, platform, ct),
                post => _filter(post, platform),
                interval);
        } 
    }
}