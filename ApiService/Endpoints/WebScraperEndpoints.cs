using ApiService.Application.Products;
using ApiService.Application.WebScrapes;
using Domain;
using Domain.Products;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Endpoints;

public static class WebScraperEndpoints
{
    public static WebApplication MapWebScraperEndpoints(this WebApplication app)
    {
        app.MapPost("/webscraper/scrape", async (
            [FromServices] IWebScrapeService webScrapeService,
            [FromServices] IProductService productService,
            [FromBody] ScrapeRequest request,
            ILogger<Program> logger) =>
        {
            var scrapeResults = await webScrapeService.ScrapeProducts(request.BatterySystem);
            var upsertRequests = scrapeResults.Select(x => x.ToUpsertProductRequest());
            List<UpsertProductResponse> upsertResponses = [];
            foreach (var upsertRequest in upsertRequests)
            {
                var upserted = await productService.UpsertProduct(upsertRequest);
                upsertResponses.Add(upserted);
            }
            
            return new ScrapeResponse(
                scrapeResults.Count,
                upsertResponses.Count(x => x.ChangeType == UpsertChangeType.Created),
                upsertResponses.Count(x => x.ChangeType == UpsertChangeType.Updated),
                scrapeResults.Select(x => x.Sku).ToList()
                );
        }).WithDefaults();
        
        return app;
    }
    
    private static RouteHandlerBuilder WithDefaults(this RouteHandlerBuilder builder)
    {
        return builder
            .WithTags("WebScraper");
    }

    private record ScrapeRequest(string BatterySystem) : IHaveExample
    {
        // M18, M12, mx-fuel
        public static object GetExample() => new ScrapeRequest("M12");
    }
    
    private record ScrapeResponse(int ScrapedCount, int InsertedCount, int UpdatedCount, List<string> Skus);
}
