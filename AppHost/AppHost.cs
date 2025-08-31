var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("PostgresSql")
    .WithDataVolume("PostgresSqlDataVolume", isReadOnly: false)
    .WithPgAdmin()
    // Use a custom container image that has pgvector installed
    .WithImage("ankane/pgvector", "latest")
    .WithAnnotation(new ContainerImageAnnotation { Image = "ankane/pgvector", Tag = "latest" })
    // Mount the database scripts into the container that will configure pgvector
    .WithBindMount("./database", "/docker-entrypoint-initdb.d", isReadOnly: true)
    .WithEndpoint("tcp", endpoint =>
    {
        endpoint.Port = 5432;
        endpoint.TargetPort = 5432;
        endpoint.IsExternal = true;
    })
    ;
var postgresDb = postgres
    .AddDatabase("PostgresRagDb");


var migrations = builder.AddProject<Projects.MigrationService>("migrations")
    .WithReference(postgresDb)
    .WaitFor(postgresDb);

var apiService = builder.AddProject<Projects.ApiService>("apiservice")
    .WithReference(postgresDb).WaitFor(postgresDb)
    .WithReference(migrations).WaitFor(migrations)
    .WithHttpHealthCheck("/health")
    ;

builder.AddProject<Projects.Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache).WaitFor(cache)
    .WithReference(apiService).WaitFor(apiService)
    ;

builder.Build().Run();
