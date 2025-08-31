using Microsoft.Extensions.Hosting;

const string PostgresDataVolumeName = "PostgresSqlDataVolume";
const string OllamaDataVolumeName = "OllamaDataVolume";

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("PostgresSql")
    .WithDataVolume(PostgresDataVolumeName, isReadOnly: false)
    .WithPgAdmin()
    // Use a custom container image that has pgvector installed
    .WithImage("ankane/pgvector", "latest")
    .WithAnnotation(new ContainerImageAnnotation { Image = "ankane/pgvector", Tag = "latest" })
    // Mount the database scripts into the container that will configure pgvector
    .WithBindMount("./database", "/docker-entrypoint-initdb.d", isReadOnly: true)
    ;

if (builder.Environment.IsDevelopment())
{
    postgres
        .WithEndpoint(name: "postgresendpoint", scheme: "tcp", port: 5432, targetPort: 5432, isProxied: false);
}

var postgresDb = postgres
    .AddDatabase("PostgresRagDb");


var migrations = builder.AddProject<Projects.MigrationService>("migrations")
    .WithReference(postgresDb)
    .WaitFor(postgresDb);

var ollama = builder.AddOllama("ollama")
        .WithDataVolume(OllamaDataVolumeName, isReadOnly: false)
        .WithContainerRuntimeArgs("--gpus=all")
    ;

var nomicEmbed = ollama.AddModel("nomic-embed-text:v1.5")
    ;

var apiService = builder.AddProject<Projects.ApiService>("apiservice")
    .WithReference(postgresDb).WaitFor(postgresDb)
    .WithReference(migrations).WaitFor(migrations)
    .WithReference(nomicEmbed, "embed-text").WaitFor(nomicEmbed)
    .WithHttpHealthCheck("/health")
    ;

builder.AddProject<Projects.Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache).WaitFor(cache)
    .WithReference(apiService).WaitFor(apiService)
    ;

builder.Build().Run();
