using ApiService.Application.Ai;
using OpenAI.Embeddings;

namespace ApiService.Infrastructure.Ai;

public class OpenAiEmbeddingDao(EmbeddingClient openAiEmbedding) : IEmbeddingDao
{
    public async Task<ReadOnlyMemory<float>> GetEmbeddingAsync(string input)
    {
        var result = await openAiEmbedding.GenerateEmbeddingAsync(input);
        return result.Value.ToFloats();
    }
}
