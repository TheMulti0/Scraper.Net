﻿using System;

namespace Scraper.Net.Facebook
{
    internal record FacebookSharedPost
    {
        public string Id { get; init; }

        public string Url { get; init; }

        public string Text { get; init; }

        public DateTime? CreationDate { get; init; }

        public Author Author { get; init; }
    }
}