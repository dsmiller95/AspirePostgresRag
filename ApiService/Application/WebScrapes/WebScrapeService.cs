using System.Text.Json;
using Domain.Extensions;
using Domain.WebScrapes;

namespace ApiService.Application.WebScrapes;

public interface IWebScrapeService
{
    IAsyncEnumerable<WebScrapedProduct> ScrapeProducts(string batterySystem);
}

public class WebScrapeService(HttpClient httpClient, ILogger<WebScrapeService> logger) : IWebScrapeService
{
    public async IAsyncEnumerable<WebScrapedProduct> ScrapeProducts(string batterySystem)
    {
        var allProducts = await GetAllProductsForBatterySystem(batterySystem);

        foreach (var product in allProducts)
        {
            if (product.Sku is null)
            {
                logger.LogWarning("Product {Title} has no SKU, skipping", product.Title);
                continue;
            }
            var productDetails = await GetProductDetails(product.Sku);
            if (productDetails is null)
            {
                logger.LogWarning("Could not get details for product with SKU {Sku}, skipping", product.Sku);
                continue;
            }
            var rawJson = JsonSerializer.Serialize(productDetails);
            var productSummary = ComposeSummary(productDetails);
            yield return new WebScrapedProduct
            {
                Sku = productDetails.Sku,
                Title = productDetails.Title ?? "No Title",
                RawJsonContent = rawJson,
                TextualSummary = productSummary,
            };
        }
    }
    
    private async Task<List<MkeToolProductSummary>> GetAllProductsForBatterySystem(string batterySystem)
    {
        var response = await httpClient.PostAsJsonAsync($"https://www.milwaukeetool.com/api/v1/products/merchandiser", new
        {
            Language = "en",
            batterySystem = batterySystem,
        });
        response.EnsureSuccessStatusCode();
        var scrapedData = await response.Content.ReadFromJsonAsync<MkeToolApiMultipleResponse<MkeToolProductSummary>>();
        if (scrapedData is null)
        {
            return [];
        }

        return scrapedData.Data.Results;
    }

    private async Task<MkeToolResponses.ProductDetails?> GetProductDetails(string sku)
    {
        var response = await httpClient.PostAsJsonAsync("https://www.milwaukeetool.com/api/v1/products/" + sku, new
        {
            IncludeDocuments = true,
            IncludeKits = true,
            Language = "en",
            Sku = sku,
        });
        response.EnsureSuccessStatusCode();
        var scrapedData = await response.Content.ReadFromJsonAsync<MkeToolApiSingleResponse<MkeToolResponses.ProductDetails>>();
        if (scrapedData?.Data.Result is null)
        {
            logger.LogWarning(
                "Could not parse product data for sku {Sku}, given content {ResponseContent}",
                sku,
                await response.Content.ReadAsStringAsync());
            return null;
        }

        return scrapedData.Data.Result;
    }
    
    private static string ComposeSummary(MkeToolResponses.ProductDetails details)
    {
        var builder = new System.Text.StringBuilder();
        builder.AppendLine(details.Sku);
        builder.AppendLine(details.Title);
        builder.AppendLine();
        builder.AppendLine(details.Overview);
        builder.AppendLine();
        builder.AppendLine("Features:");
        foreach (var feature in details.Features)
        {
            builder.AppendLine($"- {feature}");
        }

        var uniqueAltTexts = details.MediaGallery
            .Concat(details.SupportingImages)
            .Concat(details.Videos)
            .Select(x => x.Alt ?? x.Poster?.Alt)
            .Append(details.HeroImage?.Alt)
            .WhereNotNull().Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct();
        builder.AppendLine();
        builder.AppendLine("Images:");
        foreach (var altText in uniqueAltTexts)
        {
            builder.AppendLine($"- {altText}");
        }
        
        builder.AppendLine();
        builder.AppendLine("Specifications:");
        foreach (var spec in details.Specs2)
        {
            builder.AppendLine($"- {spec.Name}: {spec.Value}");
        }
        
        builder.AppendLine();
        builder.AppendLine($"product categories:");
        foreach (var category in details.MarketingCategories.Values)
        {
            builder.AppendLine($"- {category.Display}");
        }
        return builder.ToString();
    }
    
    
    private record MkeToolApiMultipleResponse<T>(string Status, string Message, MkeToolApiMultipleResponseData<T> Data);
    private record MkeToolApiMultipleResponseData<T>(List<T> Results);
    private record MkeToolApiSingleResponse<T>(string Status, string Message, MkeToolApiSingleResponseData<T> Data);
    private record MkeToolApiSingleResponseData<T>(T? Result);

    private record MkeToolProductSummary(string? Sku, string? Title);

    private static class MkeToolResponses
    {
        public class ProductDetails
        {
            public required string Sku { get; set; }
            public string? Title { get; set; }
            public string? Overview { get; set; }
            public required List<string> Features { get; set; }
            public required ProductImage? HeroImage { get; set; }
            public List<MediaGalleryItem> MediaGallery { get; set; } = [];
            public List<MediaGalleryItem> SupportingImages { get; set; } = [];
            public List<MediaGalleryItem> Videos { get; set; } = [];
            public required Dictionary<string, MarketingCategory> MarketingCategories { get; set; }
            // Specs appears to be deprecated, all data is also represented in Specs2
            //public required Dictionary<string, SpecDetail> Specs { get; set; }
            public required List<Spec2Detail> Specs2 { get; set; }
            // public required List<object> Includes { get; set; }
            // public required List<object> Variants { get; set; }
            // public required List<object> Kits { get; set; }
        }

        public class ProductImage
        {
            public required string Url { get; set; }
            public string? Alt { get; set; }
            public string? Filename { get; set; }
            public string? Type { get; set; }
        }

        public class MediaGalleryItem
        {
            public string? Url { get; set; }
            public string? Alt { get; set; }
            public string? Filename { get; set; }
            public string? Type { get; set; }
            public Poster? Poster { get; set; }
        }

        public class Poster
        {
            public required string Url { get; set; }
            public string? Alt { get; set; }
            public string? Filename { get; set; }
            public string? Type { get; set; }
        }

        public class MarketingCategory
        {
            public required string Display { get; set; }
            public string? Parent { get; set; }
            public required string Url { get; set; }
        }

        public class Spec2Detail
        {
            public required string Key { get; set; }
            public required string Name { get; set; }
            public required string Value { get; set; }
            public required string Display { get; set; }
        }
    }
}
