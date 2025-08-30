using Domain.Products;

namespace Data.Products;

public class ProductDb
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public required decimal Price { get; init; } 
    
    public Product ToDomain()
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(Id, 0);
        return new Product
        {
            Id = Id,
            Title = Title,
            Price = Price
        };
    }
    
    public static ProductDb From(Product item)
    {
        return new ProductDb
        {
            Id = item.Id,
            Title = item.Title,
            Price = item.Price
        };
    }
}
