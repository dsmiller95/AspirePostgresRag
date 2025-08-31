using Domain.Products;

namespace ApiService.Models.Products;

public class RankedProductModel
{
    public required int Id { get; init; }
    public required string UniqueSku { get; init; }
    public required string Title { get; init; }
    public required double Score { get; init; }
}

public static class RankedProductModelExtensions
{
    public static RankedProductModel ToModel(this RankedProduct product) =>
        new()
        {
            Id = product.Id,
            UniqueSku = product.UniqueSku,
            Title = product.Title,
            Score = product.Score,
        };
    
    public static IEnumerable<RankedProductModel> ToModels(this IEnumerable<RankedProduct> products) =>
        products.Select(p => p.ToModel());
}
