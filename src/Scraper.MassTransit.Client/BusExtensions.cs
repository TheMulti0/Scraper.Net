using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MassTransit;

namespace Scraper.MassTransit.Client
{
    public static class BusExtensions
    {
        public static IObservable<ConsumeContext<T>> ConnectRequestObservable<T>(
            this IBus bus,
            Guid requestId) where T : class
        {
            var subject = new Subject<ConsumeContext<T>>();
            
            var connection = bus.ConnectRequestObserver(requestId, subject);

            return subject.Finally(() => connection.Disconnect());
        }
    }
}