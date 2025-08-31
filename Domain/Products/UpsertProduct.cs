namespace Domain.Products;

public record UpsertProduct(string UniqueSku, string Title, string ScrapedJson) : IHaveExample
{
    public static object GetExample() => 
        new UpsertProduct(
            "SKU12345",
            "Drill press", 
            @"""
{
    ""productId"": ""12345"",
    ""name"": ""Drill press"",
    ""description"": ""Drilsl holes accurately and efficiently."",
    ""price"": 19.99
}
""");
    
    public bool HasChanges(Product other) =>
        other.UniqueSku != UniqueSku ||
        other.Title != Title ||
        other.ScrapedJson != ScrapedJson;
}
