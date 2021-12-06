using Scraper.Net;

namespace Scraper.MassTransit.Common
{
    public record NewPost
    {
        public Post Post { get; init; }
        public string Platform { get; init; }
        
        public override string ToString()
        {
            return $"[{Platform}] {Post.Url}";
        }
    }
}