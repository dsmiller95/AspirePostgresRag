namespace Domain.Products;

public record Product
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required decimal Price { get; init; }
}
