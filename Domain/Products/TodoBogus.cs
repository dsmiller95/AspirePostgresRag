namespace Domain.Products;

public static class ProductBogus
{
    public static List<Product> Generate(int count = 10)
    {
        var faker = new Bogus.Faker<Product>()
            .RuleFor(t => t.Id, f => 0) // Id is set by the database
            .RuleFor(t => t.Price, f => f.Finance.Amount(10, 100));

        return faker.Generate(count);
    }
}
