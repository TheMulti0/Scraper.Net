using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Stream
{
    public static class InMemoryPostTimeFilter
    {
        private static readonly Dictionary<string, DateTime> LatestPostsTimes = new();
        
        public static async Task<bool> Filter(
            Post post,
            string platform,
            CancellationToken ct = default)
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