namespace AspirePostgresRag.ApiService.Options;

public class AiOptions
{
    public string OpenAiApiKey { get; set; } = null!;
    public string ChatModel { get; set; } = "gpt-4o";
    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
}
