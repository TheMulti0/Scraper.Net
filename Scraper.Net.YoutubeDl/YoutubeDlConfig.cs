using YoutubeDLSharp.Options;

namespace Scraper.Net.YoutubeDl
{
    public record YoutubeDlConfig
    {
        public byte DegreeOfConcurrency { get; init; } = 4;

        public string YoutubeDlPath { get; init; } = "youtube-dl.exe";

        public string FfMpegPath { get; init; } = "ffmpeg.exe";
        
        public OptionSet OverrideOptions { get; init; }
    }
}