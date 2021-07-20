using YoutubeDLSharp.Options;

namespace Scraper.Net.YoutubeDl
{
    public record YoutubeDlConfig
    {
        public bool KeepReceivedPost { get; init; }
        
        public byte DegreeOfConcurrency { get; init; } = 4;

        public string YoutubeDlPath { get; init; }
#if _WINDOWS
            = "youtube-dl.exe";
#else
            = "/usr/local/bin/youtube-dl";
#endif

        public OptionSet OverrideOptions { get; init; }
    }
}