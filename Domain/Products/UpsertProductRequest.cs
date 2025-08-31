namespace Domain.Products;

public record UpsertProductRequest(string UniqueSku, string Title, string AiSummary) : IHaveExample
{
    public static object GetExample() => 
        new UpsertProductRequest(
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
        other.AiSummary != AiSummary;
}
