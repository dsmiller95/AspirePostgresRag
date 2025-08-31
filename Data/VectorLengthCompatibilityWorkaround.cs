using Pgvector;
namespace Data;

public static class VectorLengthCompatibilityWorkaround
{
    public const int EmbeddingLength = 1536;

    public static Vector EnsureValidTextEmbedding(this Vector embedding)
    {
        switch (embedding.Memory.Length)
        {
            case EmbeddingLength:
                return embedding;
            case > EmbeddingLength:
                throw new ArgumentOutOfRangeException(nameof(embedding),
                    $"Embedding length {embedding.Memory.Length} exceeds the maximum allowed length of {EmbeddingLength}.");
            case < EmbeddingLength:
                // pad with zeros
                var padded = new float[EmbeddingLength];
                embedding.Memory.Span.CopyTo(padded);
                return new Vector(padded);
        }
    }
}
