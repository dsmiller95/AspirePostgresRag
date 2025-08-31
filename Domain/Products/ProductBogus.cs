using System.Text.Json;

namespace Domain.Products;

public static class ProductBogus
{
    public static List<Product> Generate(int count = 10)
    {
        var faker = new Bogus.Faker<Product>()
            .RuleFor(t => t.Id, f => 0) // Id is set by the database
            .RuleFor(t => t.UniqueSku, f => f.Commerce.Ean13())
            .RuleFor(t => t.Title, f => f.Commerce.ProductAdjective() + " " + f.Commerce.ProductName())
            .RuleFor(t => t.ScrapedJson, f => JsonSerializer.Serialize(new
            {
                Name = f.Commerce.ProductName(),
                Description = f.Commerce.ProductDescription(),
                Price = f.Commerce.Price()
            }));

        return faker.Generate(count);
    }
}
