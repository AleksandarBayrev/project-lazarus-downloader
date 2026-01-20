using System.Net;
using System.Text.Json;
using HtmlAgilityPack;

namespace PLD
{
    public static class PdfUrlFetcher
    {
        public static async Task<string?> FetchPdfUrlAsync(string pageUrl)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(pageUrl);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(data);

                    var options = WebUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("//div[@class='real3dflipbook']")?.GetAttributeValue("data-flipbook-options", ""));

                    if (options == null)
                    {
                        return null;
                    }

                    var filteredData = JsonSerializer.Deserialize<Dictionary<string, object>>(options);

                    if (filteredData != null && filteredData.TryGetValue("pdfUrl", out var pdfUrl) && pdfUrl != null)
                    {
                        return WebUtility.HtmlDecode(pdfUrl.ToString());
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching PDF URL from the page {pageUrl}, error: {ex.Message}");
                return null;

            }
        }
    }
}