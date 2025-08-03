using AspirePostgresRag.ApiService;
using AspirePostgresRag.Data;
using AspirePostgresRag.Models.Exceptions;
using AspirePostgresRag.Models.TodoItems;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<TodoDbContext>(connectionName: "PostgresRagDb", configureDbContextOptions: o =>
{
    o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

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

app.MapGet("/todos", async (TodoDbContext db, ILogger<Program> logger) =>
{
    try
    {
        var items = await db.TodoItems.ToListAsync();
        return Results.Ok(items);
    }catch (Exception ex)
    {
        logger.LogCritical(ex, "Failed to connect to the database.");
        return Results.Ok(TodoBogus.Generate());
    }
});
    
app.MapGet("/todos/{id:int}", async (int id, TodoDbContext db) =>
{
    var item = await db.TodoItems.FindAsync(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
});

app.MapPost("/todos", async (TodoDbContext db, CreateTodoItem item) =>
{
    var dbItem = new TodoDbItem
    {
        Title = item.Title,
        IsCompleted = item.IsCompleted
    };
    var added = db.TodoItems.Add(dbItem);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{added.Entity.Id}", added.Entity.ToDomain());
});

app.MapPut("/todos/{id:int}", async (int id, TodoDbContext db, UpdateTodoItem updatedItem) =>
{
    var item = (await db.TodoItems.FindAsync(id))?.ToDomain();
    if (item is null)
    {
        return Results.NotFound();
    }

    item = item with
    {
        Title = updatedItem.Title,
        IsCompleted = updatedItem.IsCompleted,
    };
    var dbItem = TodoDbItem.FromDomain(item);
    db.TodoItems.Update(dbItem);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todos/{id:int}", async (int id, TodoDbContext db) =>
{
    var item = await db.TodoItems.FindAsync(id);
    if (item is null)
    {
        return Results.NotFound();
    }

    db.TodoItems.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
