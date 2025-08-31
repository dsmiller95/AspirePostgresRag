namespace ApiService.Options;

public class AiOptions
{
    public required AiProvider Provider { get; set; }
    
    public required OpenAiApiOptions? OpenAi { get; set; }
    public required OllamaApiOptions? Ollama { get; set; }
}

public enum AiProvider
{
    OpenAi,
    Ollama
}

public class OpenAiApiOptions
{
    public required string ApiKey { get; set; }
    public required string ChatModel { get; set; }
    public required string EmbeddingModel { get; set; }
}

public class OllamaApiOptions
{
    public required string ChatModel { get; set; }
    public required string EmbeddingModel { get; set; }
}
