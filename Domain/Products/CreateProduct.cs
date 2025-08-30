namespace Domain.Products;

public record CreateProduct(string Title, decimal Price) : IHaveExample
{
    public static object GetExample()
    {
        return new CreateProduct("Drill", 19.99m);
    }
}
