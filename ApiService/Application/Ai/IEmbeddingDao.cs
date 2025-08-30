namespace ApiService.Application.Ai;

public interface IEmbeddingDao
{
    Task<ReadOnlyMemory<float>> GetEmbeddingAsync(string input);
}
