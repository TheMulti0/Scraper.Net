using System;
using System.Collections.Generic;

namespace Scraper.Net.Stream
{
    public static class InMemoryPostTimeFilter
    {
        private static readonly Dictionary<string, DateTime> LatestPostsTimes = new();
        
        public static bool Filter(Post post, string platform)
        {
            if (post.CreationDate == null)
            {
                return false;
            }
            
            var postCreationDate = (DateTime) post.CreationDate;

            if (LatestPostsTimes.ContainsKey(platform))
            {
                if (post.CreationDate <= LatestPostsTimes[platform])
                {
                    return false;
                }
                
                LatestPostsTimes[platform] = postCreationDate;
                return true;
            }
            
            LatestPostsTimes.Add(platform, postCreationDate);
            return true;
        }
    }
}