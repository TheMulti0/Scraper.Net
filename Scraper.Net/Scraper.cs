using System;
using System.Collections.Generic;
using System.Linq;

namespace Scraper.Net
{
    public class Scraper
    {
        private readonly IDictionary<string, IPlatformScraper> _platformScrapers;
        private readonly IEnumerable<IPostProcessor> _postProcessors;

        public Scraper(
            IDictionary<string, IPlatformScraper> platformScrapers,
            IEnumerable<IPostProcessor> postProcessors)
        {
            _platformScrapers = platformScrapers;
            _postProcessors = postProcessors;
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(string id, string platform)
        {
            IPlatformScraper scraper = GetScraper(platform);
            IEnumerable<Post> scrapedPosts = await scraper.GetPostsAsync(id);
            
            IAsyncEnumerable<Post> posts = _postProcessors.Aggregate(
                scrapedPosts.ToAsyncEnumerable(),
                ProcessPosts);

            await foreach (Post processedPost in posts)
            {
                yield return processedPost;
            }
        }

        private static IAsyncEnumerable<Post> ProcessPosts(
            IAsyncEnumerable<Post> posts,
            IPostProcessor postProcessor)
        {
            return posts.SelectMany(post =>
            {
                try
                {
                    return postProcessor.ProcessAsync(post);
                }
                catch
                {
                    return new [] { post }.ToAsyncEnumerable(); // Return original post without processing if processing fails
                }
            });
        }

        private IPlatformScraper GetScraper(string platform)
        {
            if (!_platformScrapers.ContainsKey(platform))
            {
                throw new ArgumentException($"No scraper found for platform {platform}");
            }

            return _platformScrapers[platform];
        }
    }
}