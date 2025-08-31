namespace Domain.Products;

public record RankedProduct : Product
{
    public required double Score { get; init; }
    
    public static RankedProduct From(Product product, double score) => new RankedProduct
    {
        Id = product.Id,
        UniqueSku = product.UniqueSku,
        Title = product.Title,
        AiSummary = product.AiSummary,
        Score = score,
    };
}
