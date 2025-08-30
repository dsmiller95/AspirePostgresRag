using ApiService;
using ApiService.Application.Todos;
using ApiService.Extensions;
using Data;
using Domain.TodoItems;
using ServiceDefaults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<TodoDbContext>(connectionName: "PostgresRagDb", configureDbContextOptions: dbContextOptionsBuilder  =>
{
    dbContextOptionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    dbContextOptionsBuilder.UseNpgsql(npgBuilder =>
    {
        npgBuilder.UseVector();
    });
});

builder.Services
    .AddAi(builder.Configuration)
    .AddApplicationServices();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/todos", async ([FromServices] ITodoService service, ILogger<Program> logger) =>
{
    try
    {
        return Results.Ok(await service.GetTodos());
    }catch (Exception ex)
    {
        logger.LogCritical(ex, "Failed to connect to the database.");
        return Results.Ok(TodoBogus.Generate());
    }
});
app.MapGet("/todos/search", async ([FromQuery] string query, [FromServices] ITodoService service) =>
{
    return Results.Ok(await service.SearchTodos(query));
});
    
app.MapGet("/todos/{id:int}", async (int id, [FromServices] ITodoService service) =>
{
    var item = await service.GetTodo(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
});

app.MapPost("/todos", async ([FromServices] ITodoService service, CreateTodoItem item) =>
{
    var added = await service.CreateTodo(item);
    return Results.Created($"/todos/{added.Id}", added);
});

app.MapPut("/todos/{id:int}", async (int id, [FromServices] ITodoService service, UpdateTodoItemCompleted updatedItemCompleted) =>
{
    var updated = await service.UpdateTodo(id, updatedItemCompleted);
    return Results.Ok(updated);
});

app.MapDelete("/todos/{id:int}", async (int id, [FromServices] ITodoService service) =>
{
    await service.DeleteTodo(id);
    return Results.NoContent();
});


app.MapDefaultEndpoints();

app.Run();

namespace ApiService
{
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
