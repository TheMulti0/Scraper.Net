using System.Threading.Tasks;
using MassTransit;
using Scraper.MassTransit.Common;

namespace PostsListener.Client.Tests
{
    internal class NewPostConsumer : IConsumer<NewPost>
    {
        private readonly NewPostCounter _counter;

        public NewPostConsumer(NewPostCounter counter)
        {
            _counter = counter;
        }

        public Task Consume(ConsumeContext<NewPost> context)
        {
            _counter.Increment();
            
            return Task.CompletedTask;
        }
    }
}