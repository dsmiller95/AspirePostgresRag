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
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Failed to connect to the database.");
                return Results.Ok(ProductBogus.Generate());
            }
        }).WithDefaults();
        
        app.MapGet("/products/search", async ([FromQuery] string query, [FromServices] IProductService service) =>
        {
            return Results.Ok(await service.SearchProducts(query));
        }).WithDefaults();
    
        app.MapGet("/products/{id:int}", async (int id, [FromServices] IProductService service) =>
        {
            var item = await service.GetProduct(id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        }).WithDefaults();

        app.MapPut("/products", async ([FromServices] IProductService service, UpsertProductRequest item) =>
        {
            var added = await service.UpsertProduct(item);
            return Results.Created($"/products/{added.Record.Id}", added);
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
}
