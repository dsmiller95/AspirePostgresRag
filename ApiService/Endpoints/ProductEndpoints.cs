using ApiService.Application.Products;
using Domain.Products;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Endpoints;

public static class ProductEndpoints
{
    public static WebApplication MapProductEndpoints(this WebApplication app)
    {
        app.MapGet("/products", async ([FromServices] IProductService service, ILogger<Program> logger) =>
        {
            try
            {
                return Results.Ok(await service.GetProducts());
            }catch (Exception ex)
            {
                logger.LogCritical(ex, "Failed to connect to the database.");
                return Results.Ok(ProductBogus.Generate());
            }
        });
    
        app.MapGet("/products/{id:int}", async (int id, [FromServices] IProductService service) =>
        {
            var item = await service.GetProduct(id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        });

        app.MapPost("/products", async ([FromServices] IProductService service, CreateProduct item) =>
        {
            var added = await service.CreateProduct(item);
            return Results.Created($"/products/{added.Id}", added);
        });

        app.MapPut("/products/{id:int}", async (int id, [FromServices] IProductService service, UpdateProductModel updateRequest) =>
        {
            var updated = await service.UpdateProduct(updateRequest.ToDomain(id));
            return Results.Ok(updated);
        });

        app.MapDelete("/products/{id:int}", async (int id, [FromServices] IProductService service) =>
        {
            await service.DeleteProduct(id);
            return Results.NoContent();
        });

        return app;
    }

    private record UpdateProductModel(string Title, decimal Price)
    {
        public UpdateProduct ToDomain(int id) => new(id, Title, Price);
    }
}
