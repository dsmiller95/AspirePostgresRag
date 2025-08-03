using AspirePostgresRag.Data;
using AspirePostgresRag.MigrationService;
using AspirePostgresRag.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));
builder.AddNpgsqlDbContext<TodoDbContext>(connectionName: "PostgresRagDb", configureDbContextOptions: o =>
{
    o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

var host = builder.Build();
host.Run();
