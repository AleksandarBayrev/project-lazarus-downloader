namespace PLD
{
    public class DownloadStatus
    {
        public string PageUrl { get; set; } = string.Empty;
        public bool IsDownloaded { get; set; }
        public string? ResultFileName { get; set; } = string.Empty;
        public string? FileLocation { get; set; } = string.Empty;
    }
}