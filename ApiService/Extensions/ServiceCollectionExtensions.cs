using System.Diagnostics;
using ApiService.Application.Ai;
using ApiService.Application.Products;
using ApiService.Application.Todos;
using ApiService.Infrastructure.Ai;
using ApiService.Options;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using OpenAI.Embeddings;

namespace ApiService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddAi(this IHostApplicationBuilder builder)
    {
        var provider = builder.Configuration.GetRequiredEnum<AiProvider>("AI:Provider");
        switch (provider)
        {
            case AiProvider.OpenAi:
                builder.Services.AddOptions<OpenAiApiOptions>().BindConfiguration("AI:OpenAi")
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
                builder.Services.AddOpenAi(builder.Configuration);
                break;
            case AiProvider.Ollama:
                builder.Services.AddOptions<OllamaApiOptions>().BindConfiguration("AI:Ollama")
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
                builder.AddOllama();
                break;
            default:
                throw new UnreachableException();
        }
        
        return builder;
    }

    private static IServiceCollection AddOpenAi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ChatClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<OpenAiApiOptions>>().Value;

            return new ChatClient(options.ChatModel, options.ApiKey);
        });
        services.AddSingleton<EmbeddingClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<OpenAiApiOptions>>().Value;

            return new EmbeddingClient(options.EmbeddingModel, options.ApiKey);
        });
        
        services.AddScoped<IEmbeddingDao, OpenAiEmbeddingDao>();
        
        return services;
    }
    
    private static IHostApplicationBuilder AddOllama(this IHostApplicationBuilder builder)
    {
        builder.AddKeyedOllamaApiClient(
            serviceKey: AiServiceKeys.EmbeddingClient,
            connectionName: "embed-text");
        builder.Services.AddScoped<IEmbeddingDao, OllamaEmbeddingDao>();
        
        return builder;
    }
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IEmbeddingService, AiEmbeddingService>();
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<IProductService, ProductService>();
        

        return services;
    }
}
