using ApiService.Application.Products;
using Domain;
using Domain.Products;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Endpoints;

public static class ProductEndpoints
{
    public static WebApplication MapProductEndpoints(this WebApplication app)
    {
        app.MapGet("/products", async ([FromServices] IProductService service) =>
            Results.Ok(await service.GetProducts()))
            .WithDefaults();
    
        app.MapGet("/products/{id:int}", async (int id, [FromServices] IProductService service) => 
            await service.GetProduct(id) ?? throw new Exception("not found"))
            .WithDefaults();

        app.MapPost("/products", async ([FromServices] IProductService service, CreateProduct item) =>
        {
            var added = await service.CreateProduct(item);
            return Results.Created($"/products/{added.Id}", added);
        }).WithDefaults();
        
        app.MapPost("/products/bogus/{count}", async ([FromServices] IProductService service, int count) =>
        {
            var added = await service.GenerateProducts(count);
            return Results.Created($"/products/", added);
        }).WithDefaults();

        app.MapPut("/products/{id:int}", async (int id, [FromServices] IProductService service, UpdateProductModel updateRequest) =>
        {
            var updated = await service.UpdateProduct(updateRequest.ToDomain(id));
            return Results.Ok(updated);
        }).WithDefaults();

        app.MapDelete("/products/{id:int}", async (int id, [FromServices] IProductService service) =>
        {
            await service.DeleteProduct(id);
            return Results.NoContent();
        }).WithDefaults();

        return app;
    }
    
    private static RouteHandlerBuilder WithDefaults(this RouteHandlerBuilder builder)
    {
        return builder
            .WithTags("Products");
    }

    private record UpdateProductModel(string Title, decimal Price) : IHaveExample
    {
        public UpdateProduct ToDomain(int id) => new(id, Title, Price);
        public static object GetExample()
        {
            return new UpdateProductModel("Drill", 19.99m);
        }
    }
}
