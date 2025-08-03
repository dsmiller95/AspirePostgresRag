using Pgvector;

namespace AspirePostgresRag.ApiService.Application.Ai;

public interface IEmbeddingService
{
    Task<ReadOnlyMemory<float>> GetEmbeddingAsync(string input);
}

public class AiEmbeddingService(IEmbeddingDao embeddingDao) : IEmbeddingService
{
    public async Task<ReadOnlyMemory<float>> GetEmbeddingAsync(string input)
    {
        return await embeddingDao.GetEmbeddingAsync(input);
    }
}
