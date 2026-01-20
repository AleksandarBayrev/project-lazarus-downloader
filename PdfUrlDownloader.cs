namespace PLD
{
    public static class PdfUrlDownloader
    {
        public static async Task<bool> DownloadPdfAsync(string pdfUrl, string destinationPath)
        {
            try
            {
                using var client = new HttpClient();

                var response = await client.GetAsync(pdfUrl);

                if (response.IsSuccessStatusCode)
                {
                    var pdfData = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(destinationPath, pdfData);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error downloading PDF from {pdfUrl}, error: {ex.Message}");
                return false;
            }
        }
    }
}