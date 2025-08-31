using Domain.Products;

namespace ApiService.Models.Products;

public class ProductModel
{
    public required int Id { get; init; }
    public required string UniqueSku { get; init; }
    public required string Title { get; init; }
}

public static class ProductModelExtensions
{
    public static ProductModel ToModel(this Product product) =>
        new()
        {
            Id = product.Id,
            UniqueSku = product.UniqueSku,
            Title = product.Title,
        };
    
    public static IEnumerable<ProductModel> ToModels(this IEnumerable<Product> products) =>
        products.Select(p => p.ToModel());
}
