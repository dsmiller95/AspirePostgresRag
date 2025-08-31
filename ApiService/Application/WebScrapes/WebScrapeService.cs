using System.Text.Json;
using Domain.Extensions;
using Domain.WebScrapes;

namespace ApiService.Application.WebScrapes;

public interface IWebScrapeService
{
    Task<List<WebScrapedProduct>> ScrapeProducts(string batterySystem);
}

public class WebScrapeService(HttpClient httpClient, ILogger<WebScrapeService> logger) : IWebScrapeService
{
    public async Task<List<WebScrapedProduct>> ScrapeProducts(string batterySystem)
    {
        var response = await httpClient.PostAsJsonAsync($"https://www.milwaukeetool.com/api/v1/products/merchandiser", new
        {
            Language = "en",
            batterySystem = batterySystem,
        });
        response.EnsureSuccessStatusCode();
        var scrapedData = await response.Content.ReadFromJsonAsync<MkeToolMerchandiserResponse>();
        if (scrapedData is null)
        {
            return [];
        }
        
        return scrapedData.Data.Results
            .Select(x =>
            {
                var rawText = x.GetRawText();
                var deserialized = x.Deserialize<MkeToolProductParser>(JsonSerializerOptions.Web);
                if (deserialized?.Sku == null)
                {
                    logger.LogWarning("Sku is null on content {ProductContent}", rawText);
                    return null;
                }

                return new WebScrapedProduct
                {
                    Sku = deserialized.Sku, 
                    Title = deserialized.Title ?? "No Title", 
                    RawJsonContent = rawText,
                };
            })
            .WhereNotNull()
            .ToList();
    }
    
    private record MkeToolMerchandiserResponse(string Status, string Message, MkeToolResultResponse Data);
    private record MkeToolResultResponse(List<JsonElement> Results);

    private record MkeToolProductParser(string? Sku, string? Title);

}
