using ApiService.Application.Ai;
using OllamaSharp;
using OllamaSharp.Models;

namespace ApiService.Infrastructure.Ai;

public class OllamaEmbeddingDao(
    [FromKeyedServices(AiServiceKeys.EmbeddingClient)] IOllamaApiClient ollamaEmbedding
    ) : IEmbeddingDao
{
    public async Task<ReadOnlyMemory<float>> GetEmbeddingAsync(string input)
    {
        var embedRequest = new EmbedRequest()
        {
            Input = [input],
        };
        var response = await ollamaEmbedding.EmbedAsync(embedRequest);
        return response.Embeddings[0];
    }
}
