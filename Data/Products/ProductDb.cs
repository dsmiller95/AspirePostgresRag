using Domain.Products;
using Pgvector;

namespace Data.Products;

public class ProductDb
{
    private ProductDb() { }
    
    public int Id { get; init; }
    public required string UniqueSku { get; init; }
    public required string Title { get; init; }
    public required string ScrapedJson { get; init; }
    
    public required Vector Embedding { get; set; }
    
    public Product ToDomain()
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(Id, 0);
        return new Product
        {
            Id = Id,
            UniqueSku = UniqueSku,
            Title = Title,
            ScrapedJson = ScrapedJson,
        };
    }
    
    public static ProductDb From(UpsertProduct upsertRequest, Vector embedding)
    {
        return new ProductDb
        {
            UniqueSku = upsertRequest.UniqueSku,
            Title = upsertRequest.Title,
            ScrapedJson = upsertRequest.ScrapedJson,
            Embedding = embedding.EnsureValidTextEmbedding(),
        };
    }
    
    public static ProductDb From(Product item, Vector embedding)
    {
        return new ProductDb
        {
            Id = item.Id,
            UniqueSku = item.UniqueSku,
            Title = item.Title,
            ScrapedJson = item.ScrapedJson,
            Embedding = embedding.EnsureValidTextEmbedding(),
        };
    }
}
