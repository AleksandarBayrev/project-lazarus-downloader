using System.Text.Json;

namespace PLD
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var filePath = args.Length > 0 ? args[0] : null;

            Console.WriteLine(filePath);

            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("Please provide a valid file path as a command-line argument.");
                Console.WriteLine("Example: project-lazarus-downloader.exe urls.json or project-lazarus-downloader /home/aleksandar/Downloads/urls.json");
                return;
            }

            var adjustedFilePath = Path.GetFullPath(filePath);

            if (!File.Exists(adjustedFilePath) || !File.GetAttributes(adjustedFilePath).HasFlag(FileAttributes.Normal))
            {
                Console.WriteLine($"The specified file {adjustedFilePath} does not exist.");
                return;
            }

            var listOfUrls = new List<string>();
            try
            {
                listOfUrls.AddRange(JsonSerializer.Deserialize<List<string>>(await File.ReadAllTextAsync(adjustedFilePath)) ?? new List<string>());
            }
            catch (JsonException)
            {
                Console.WriteLine($"The file {adjustedFilePath} is not a valid JSON file.");
                return;
            }

            if (listOfUrls.Count == 0)
            {
                Console.WriteLine("The list of URLs is empty.");
                return;
            }

            Console.WriteLine($"Successfully read the list of URLs from path {adjustedFilePath}.");

            var downloadStatus = new Dictionary<string, DownloadStatus>();
            Console.WriteLine($"Found {listOfUrls.Count} URLs to process.");

            foreach (var url in listOfUrls)
            {
                try
                {
                    Console.WriteLine($"Processing URL: {url}");
                    var pdfUrl = await PdfUrlFetcher.FetchPdfUrlAsync(url);

                    if (pdfUrl == null)
                    {
                        throw new Exception($"No PDF URL found on the page {url}.");
                    }

                    var resultFile = pdfUrl.Split('/').Last();
                    Console.WriteLine($"Downloading PDF from: {pdfUrl} to {resultFile}");

                    var result = await PdfUrlDownloader.DownloadPdfAsync(pdfUrl, resultFile);

                    if (!result)
                    {
                        Console.WriteLine($"Failed to download the PDF from {pdfUrl}.");
                        continue;
                    }

                    Console.WriteLine($"PDF {resultFile} downloaded successfully.");
                    downloadStatus[url] = new DownloadStatus
                    {
                        PageUrl = url,
                        IsDownloaded = true,
                        ResultFileName = resultFile
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    downloadStatus[url] = new DownloadStatus
                    {
                        PageUrl = url,
                        IsDownloaded = false,
                        ResultFileName = null
                    };
                }
            }

            Console.WriteLine("Download Summary:");
            foreach (var status in downloadStatus)
            {
                Console.WriteLine($"{status.Key} - {(status.Value.IsDownloaded && status.Value.ResultFileName != null ? $"Success, file name: {status.Value.ResultFileName}" : $"Failed, PDF unavailable for {status.Value.PageUrl}")}");
            }
        }
    }
}