using ApiService.Endpoints;
using ApiService.Extensions;
using ApiService.Infrastructure;
using Data;
using ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<AppDbContext>(connectionName: "PostgresRagDb", configureDbContextOptions: dbContextOptionsBuilder  =>
{
    dbContextOptionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    dbContextOptionsBuilder.UseNpgsql(npgBuilder =>
    {
        npgBuilder.UseVector();
    });
});

builder.AddAi();
builder.Services
    .AddApplicationServices();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(o =>
{
    o.AddSchemaTransformer<ExampleSchemaTransformer>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.Map("/", () => Results.Redirect("/scalar/v1"))
        .WithName("RootRedirect")
        .WithSummary("Redirect to Scalar API");
}

app
    .MapWeatherEndpoints()
    .MapTodoItemEndpoints()
    .MapProductEndpoints()
    .MapWebScraperEndpoints()
    .MapDefaultEndpoints();

app.Run();
