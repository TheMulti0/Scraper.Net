using System;
using System.Collections.Generic;
using System.Threading;
using Scraper.Net;
using Scraper.Net.Stream;

namespace PostsListener
{
    public class PostSubscription
    {
        private readonly IPostStream _stream;
        private readonly IDisposable _subscription;

        public IObservable<DateTime?> DueTime => _stream.DueTime;

        public PostSubscription(IPostStream stream, IDisposable subscription)
        {
            _stream = stream;
            _subscription = subscription;
        }

        public IAsyncEnumerable<Post> UpdateAsync(CancellationToken ct)
        {
            return _stream.UpdateAsync(ct);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}