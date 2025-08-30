using System.Text.Json;
using System.Text.Json.Serialization;
using Domain;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace ApiService.Infrastructure;

class ExampleSchemaTransformer : IOpenApiSchemaTransformer
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        NewLine = "\n",
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    };
    
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var systemType = context.JsonTypeInfo.Type;
        if(!systemType.IsAssignableTo(typeof(IHaveExample)))
        {
            return Task.CompletedTask;
        }
        
        var example = systemType.GetMethod("GetExample", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.Invoke(null, null);
        schema.Example = JsonSerializer.Serialize(example, _jsonSerializerOptions);

        return Task.CompletedTask;
    }
}
