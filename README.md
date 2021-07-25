# Scraper.Net

## What is Scraper.Net?

Scraper.Net is a multi-platform, asynchronous .NET library responsible of providing easy post scraping over a variety of sources.

## Where to begin?

Refer to the [documentation site]("https://scraper-net.github.io/Scraper.Net/) which contains guides along with an API reference.

## Simple Usage

This example is taken from the [Getting started](https://scraper-net.github.io/Scraper.Net/articles/getting_started.html) page in the documentation site.

For background explanation and breakdown of the code refer to the page.

```cs
var provider = new ServiceCollection()
    .AddLogging()
    .AddScraper(builder => builder.AddFacebook())
    .BuildServiceProvider();

var service = provider.GetRequiredService<IScraperService>();

var posts = service.GetPostsAsync("NaftaliBennett", "facebook");

await foreach (var post in posts)
{
    Console.WriteLine(post.Content);
}
```

### Roadmap:

- [x] Facebook support
- [x] Twitter support
- [x] Higher video extraction using Youtube-DL
- [x] Asynchronous main scraper
- [x] Screenshot support using Html/CSS to Image
- [x] Easy configuration of external binaries
- [x] Facebook generator support
- [x] Twitter page size investigation
- [x] RSS feeds support
- [x] Add GetProfile for scrapers
- [x] Rate-limit handling
- [ ] Proxy support
- [ ] Instagram support
- [ ] Youtube support
- [ ] Spotify support
- [ ] Telegram support