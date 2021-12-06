using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Scraper.Net;
using Scraper.MassTransit.Common;

namespace Scraper.MassTransit.Client
{
    internal class ScraperMassTransitClient : IScraperService
    {
        private readonly IRequestClient<GetAuthor> _getAuthor;
        private readonly IRequestClient<GetPosts> _getPosts;
        private readonly IBus _bus;

        public ScraperMassTransitClient(
            IBus bus,
            TimeSpan? getPostsTimeout)
        {
            _bus = bus;
            _getAuthor = bus.CreateRequestClient<GetAuthor>();
            _getPosts = bus.CreateRequestClient<GetPosts>(getPostsTimeout ?? TimeSpan.FromDays(1));
        }

        public async Task<Author> GetAuthorAsync(
            string id,
            string platform,
            CancellationToken ct = default)
        {
            var request = new GetAuthor
            {
                Id = id,
                Platform = platform
            };

            Response<Author> response = await _getAuthor.GetResponse<Author>(request, ct);

            return response.Message;
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            string platform,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            var request = new GetPosts
            {
                Id = id,
                Platform = platform
            };

            RequestHandle<GetPosts> requestHandle = _getPosts.Create(request, ct);
            
            IAsyncEnumerable<Post> posts = GetPostsObservable(requestHandle).ToAsyncEnumerable();

            await foreach (Post post in posts.WithCancellation(ct))
            {
                yield return post;
            }
        }

        private IObservable<Post> GetPostsObservable(RequestHandle requestHandle)
        {
            IObservable<Response<OperationSucceeded>> completeSignal =
                Observable.FromAsync(() => requestHandle.GetResponse<OperationSucceeded>());

            IObservable<Post> posts = _bus
                .ConnectRequestObservable<Post>(requestHandle.RequestId)
                .Select(context => context.Message);
            
            return posts
                .TakeUntil(completeSignal);
        }
    }
}