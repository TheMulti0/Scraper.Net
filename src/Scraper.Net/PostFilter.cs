namespace Scraper.Net
{
    /// <summary>
    /// Filters a post
    /// </summary>
    /// <param name="post">A post to filter</param>
    /// <param name="platform">The platform of the post</param>
    public delegate bool PostFilter(Post post, string platform);
}