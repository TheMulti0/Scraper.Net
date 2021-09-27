---
uid: articles.introduction
---

# Introduction

Scraper.Net is the base project containing the main services and abstractions.

Platform specific scraping capabilities are implemented in separate projects (which all reference the base project), or can be self implemented.

Without a platform specific implementation the base project supplies no real functionality.

Although it is possible to use this library without a DI container, there is a first-class support for Microsoft's DI container which simplifies the process of instanciating the scraper and its dependencies.

***

## Scraping

The class you should use for scraping is [`IScraperService`](xref:Scraper.Net.IScraperService).

It wraps multiple platform scrapers, post-filters and post-processors (more on those later).

Each method requires specifying the identifier you wish to scrape, and the platform identifier (both strings).

For example, if I want to scrape a Twitter user called `@POTUS`, I would include in my method call the id `POTUS` and platform id `twitter`.

With that said, the officially implemented libraries include extension methods to the scraper service that don't require specifying the platform id.

For example instead of calling: `service.GetPostsAsync("mychannelid", "youtube")`, I can `service.GetYoutubePosts("mychannelid")`

> Note: The id of each platform scraper is specified within the construction of a ScraperService and can be decided.

### Implemented platforms

The official platforms implemented are:

| Package                                                                     	| Description                                                                                                                              	|
|-----------------------------------------------------------------------------	|------------------------------------------------------------------------------------------------------------------------------------------	|
| [Scraper.Net.Facebook](https://www.nuget.org/packages/Scraper.Net.Facebook) 	| Uses the python library [facebook-scraper](https://github.com/kevinzg/facebook-scraper) to provide facebook pages scraping capabilities. 	|
| [Scraper.Net.Twitter](https://www.nuget.org/packages/Scraper.Net.Twitter)   	| Scrapes from Twitter using Twitter's official v1 API.                                                                                    	|
| [Scraper.Net.Feeds](https://www.nuget.org/packages/Scraper.Net.Feeds)       	| Scrapes feeds that support the SyndicationFeed standard (Atom/RSS 1.0/2.0).                                                              	|
| [Scraper.Net.Youtube](https://www.nuget.org/packages/Scraper.Net.Youtube)   	| Scrapes Youtube videos and channels using Google's YouTube v3 Data API.                                                                  	|



In order to create an implementation, all you have to do is implement the [`IPlatformScraper`](xref:Scraper.Net.IPlatformScraper) interface and register your custom scraper's instance in your service.

***

## Post filtering

It is possible to include asynchronous post filters that receive a [Post](xref:Scraper.Net.Post) and platform and return true if the scraped post should be returned to the caller.

> See [PostFilter](xref:Scraper.Net.PostFilter)

***

## Post processing

It is also possible to register a post processor.

The purpose of this class is to receive a post and asynchronously produce one or more posts.

### Implemented post processors

The official post processors implemented are:

| Package                                                                         	| Description                                                                                                                                  	|
|---------------------------------------------------------------------------------	|----------------------------------------------------------------------------------------------------------------------------------------------	|
| [Scraper.Net.YoutubeDl](https://www.nuget.org/packages/Scraper.Net.YoutubeDl)   	| Uses the python library [youtube-dl](https://github.com/ytdl-org/youtube-dl) to extract higher quality videos for posts that include videos. 	|
| [Scraper.Net.Screenshot](https://www.nuget.org/packages/Scraper.Net.Screenshot) 	| Uses [HTML/CSS to Image API](https://htmlcsstoimage.com/) to screenshot post's webpages.                                                     	|

> See [IPostProcessor](xref:Scraper.Net.IPostProcessor)

