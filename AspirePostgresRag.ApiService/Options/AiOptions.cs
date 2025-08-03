namespace AspirePostgresRag.ApiService.Options;

public class AiOptions
{
    public required string OpenAiApiKey { get; set; }
    public required string ChatModel { get; set; }
    public required string EmbeddingModel { get; set; }
}
