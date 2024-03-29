# Scraper.Net

## What is Scraper.Net?

Scraper.Net is a multi-platform, asynchronous .NET library responsible of providing easy post scraping over a variety of sources.

## Supported features

- [x] Scraping [Facebook](https://www.facebook.com/) pages
- [x] Scraping [RSS](https://en.wikipedia.org/wiki/RSS) feeds
- [x] Scraping [Twitter](https://twitter.com/) users
- [x] Scraping [Youtube](https://www.youtube.com/) channels
- [x] Downloading high-quality videos via [youtube-dl](https://github.com/ytdl-org/youtube-dl)
- [x] Screenshotting pages via [HtmlCssToImage](https://htmlcsstoimage.com/)
- [x] Communication with remote scraper nodes via [MassTransit](http://masstransit-project.com/)
- [x] Persisting posts (via [MongoDB](https://www.mongodb.com/)) and listening for new posts events

## Where to begin?

Refer to the [documentation site](https://themulti0.github.io/Scraper.Net) which contains [guides](https://themulti0.github.io/Scraper.Net/articles/getting_started.html) along with an [API reference](https://themulti0.github.io/Scraper.Net/api/index.html).

## Simple Usage

This example is taken from the [Getting started](https://themulti0.github.io/Scraper.Net/articles/getting_started.html) page in the documentation site.

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
