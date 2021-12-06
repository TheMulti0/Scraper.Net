namespace Scraper.MassTransit.Common
{
    public record OperationSucceeded
    {
        public static OperationSucceeded Instance { get; } = new();
    }
}