using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Scraper.Net;

namespace LogParser
{
    internal record LogEntry(DateTime Date, string Level, int ThreadId, string Category, bool Finished, string IdPlatform);
    
    internal record ScrapingProcess(string Id, string Platform, TimeSpan Duration);

    internal record PlatformStats(string Platform, TimeSpan? AverageDuration, TimeSpan? MinDuration, TimeSpan? MaxDuration);

    public class Program
    {
        private const string LogEntryPattern =
            @"\[(?<date>.+)\]\s\[(?<level>.+)\]\s\[Thread\s(?<thread_id>.+)\]\s\[(?<category>[\w.]+)\]\s(?<status>Beginning|Finished)\s(to\sscrape|scraping)\s(?<text>.+)";

        private const string ScrapingProcessPattern = @"\[(?<platform>\w+)\]\s(?<id>.+)";
        
        private readonly Regex _logEntryRegex = new(LogEntryPattern);
        private readonly Regex _scrapingProcessPattern = new(ScrapingProcessPattern);
        
        public static void Main(string[] args)
        {
            new Program().Main();
        }

        private void Main()
        {
            IEnumerable<LogEntry> logEntries = File.ReadAllLines("../../../log.txt")
                .Select(line => _logEntryRegex.Match(line))
                .Where(match => match.Success)
                .Select(ToLogEntry)
                .Where(entry => entry.Category == "Scraper.Net.Stream.PostsStreamer");

            Dictionary<string, List<DateTime>> dict = new();
            
            foreach (LogEntry logEntry in logEntries)
            {
                if (!dict.ContainsKey(logEntry.IdPlatform))
                {
                    dict.Add(logEntry.IdPlatform, new List<DateTime>
                    {
                        logEntry.Date
                    });
                }
                else
                {
                    dict[logEntry.IdPlatform].Add(logEntry.Date);
                }
            }

            IEnumerable<ScrapingProcess> scrapingProcesses = dict
                .Select(ToScrapingProcess)
                .ToList();

            var stats = scrapingProcesses
                .GroupBy(
                    process => process.Platform,
                    process => process.Duration)
                .Select(spans => new PlatformStats(
                            spans.Key,
                            spans.Aggregate((lhs, rhs) => lhs + rhs) / spans.Count(),
                            spans.Min(),
                            spans.Max()));

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new NullableTimeSpanConverter() }
            };
            
            Console.WriteLine(JsonSerializer.Serialize(stats, options));
        }

        private ScrapingProcess ToScrapingProcess(KeyValuePair<string, List<DateTime>> pair)
        {
            (string key, List<DateTime> value) = pair;

            Match match = _scrapingProcessPattern.Match(key);

            return new ScrapingProcess(
                match.Groups["id"].Value,
                match.Groups["platform"].Value,
                value[1] - value[0]);
        }

        private LogEntry ToLogEntry(Match match)
        {
            DateTime date = DateTime.Parse(match.Groups["date"].Value);
            string level = match.Groups["level"].Value;
            int threadId = int.Parse(match.Groups["thread_id"].Value);
            string category = match.Groups["category"].Value;
            bool finished = match.Groups["status"].Value == "Finished";
            string text = match.Groups["text"].Value;

            return new LogEntry(date, level, threadId, category, finished, text);
        }
    }
}