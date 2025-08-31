namespace Domain.Products;

public record UpsertProductResponse(Product Record, UpsertChangeType ChangeType);
