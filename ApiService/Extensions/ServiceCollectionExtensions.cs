using ApiService.Application.Ai;
using ApiService.Application.ErrorRecovery;
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
    public static IServiceCollection AddAi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AiOptions>().BindConfiguration("AI");
        services.AddSingleton<ChatClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<AiOptions>>().Value;

            return new ChatClient(options.ChatModel, options.OpenAiApiKey);
        });
        services.AddSingleton<EmbeddingClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<AiOptions>>().Value;

            return new EmbeddingClient(options.EmbeddingModel, options.OpenAiApiKey);
        });
        
        return services;
    }
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IEmbeddingService, AiEmbeddingService>();
        
        services.AddScoped<IEmbeddingDao, OpenAiEmbeddingDao>();
        
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IErrorRecoveryService, ErrorRecoveryService>();

        return services;
    }
}
