using Domain.Products;

namespace Domain.WebScrapes;

public class WebScrapedProduct
{
    public required string Sku { get; init; }
    public required string Title { get; init; }
    public required string RawJsonContent { get; init; }
    
    public UpsertProductRequest ToUpsertProductRequest() =>
        new UpsertProductRequest(Sku, Title, RawJsonContent);
}
