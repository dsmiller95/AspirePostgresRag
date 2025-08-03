var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("PostgresSql")
    .WithDataVolume("PostgresSqlDataVolume", isReadOnly: false)
    .WithPgAdmin();
var postgresDb = postgres
    .AddDatabase("PostgresRagDb");

var apiService = builder.AddProject<Projects.AspirePostgresRag_ApiService>("apiservice")
    .WithReference(postgresDb).WaitFor(postgresDb)
    .WithHttpHealthCheck("/health")
    ;

builder.AddProject<Projects.AspirePostgresRag_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache).WaitFor(cache)
    .WithReference(apiService).WaitFor(apiService)
    ;

builder.Build().Run();
