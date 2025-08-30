using Data;
using MigrationService;
using ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));
builder.AddNpgsqlDbContext<TodoDbContext>(connectionName: "PostgresRagDb", configureDbContextOptions: dbContextOptionsBuilder  =>
{
    dbContextOptionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    dbContextOptionsBuilder.UseNpgsql(npgBuilder =>
    {
        npgBuilder.UseVector();
    });
});

var host = builder.Build();
host.Run();
