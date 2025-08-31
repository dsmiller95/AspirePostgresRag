using ApiService.Application.Todos;
using Domain.TodoItems;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Endpoints;

public static class TodoItemEndpoints
{
    public static WebApplication MapTodoItemEndpoints(this WebApplication app)
    {
        app.MapGet("/todos", async ([FromServices] ITodoService service, ILogger<Program> logger) =>
        {
            try
            {
                return Results.Ok(await service.GetTodos());
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Failed to connect to the database.");
                return Results.Ok(TodoBogus.Generate());
            }
        }).WithDefaults();
        app.MapGet("/todos/search", async ([FromQuery] string query, [FromServices] ITodoService service) =>
        {
            return Results.Ok(await service.SearchTodos(query));
        }).WithDefaults();
    
        app.MapGet("/todos/{id:int}", async (int id, [FromServices] ITodoService service) =>
        {
            var item = await service.GetTodo(id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        }).WithDefaults();

        app.MapPost("/todos", async ([FromServices] ITodoService service, CreateTodoItem item) =>
        {
            var added = await service.CreateTodo(item);
            return Results.Created($"/todos/{added.Id}", added);
        }).WithDefaults();

        app.MapPut("/todos/{id:int}", async (int id, [FromServices] ITodoService service, UpdateTodoItemCompleted updatedItemCompleted) =>
        {
            var updated = await service.UpdateTodo(id, updatedItemCompleted);
            return Results.Ok(updated);
        }).WithDefaults();

        app.MapDelete("/todos/{id:int}", async (int id, [FromServices] ITodoService service) =>
        {
            await service.DeleteTodo(id);
            return Results.NoContent();
        }).WithDefaults();

        return app;
    }
    
    private static RouteHandlerBuilder WithDefaults(this RouteHandlerBuilder builder)
    {
        return builder
            .WithTags("TodoItems");
    }
}
