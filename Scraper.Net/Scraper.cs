using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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

        public async IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            string platform,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            IPlatformScraper scraper = GetScraper(platform);
            
            IAsyncEnumerable<Post> scrapedPosts = scraper.GetPostsAsync(id, ct);
            
            IAsyncEnumerable<Post> posts = _postProcessors.Aggregate(
                scrapedPosts,
                ProcessPosts);

            await foreach (Post processedPost in posts.WithCancellation(ct))
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