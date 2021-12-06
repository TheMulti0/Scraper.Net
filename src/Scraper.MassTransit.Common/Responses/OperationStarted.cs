namespace Scraper.MassTransit.Common
{
    public record OperationStarted
    {
        public static OperationStarted Instance { get; } = new();
    }
}