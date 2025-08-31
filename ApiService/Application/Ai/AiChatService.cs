using OpenAI.Chat;

namespace ApiService.Application.Ai;

public interface IAiChatService
{
    Task<string> GetChatResponse(string input);
}

public class AiChatService(ChatClient chatClient) : IAiChatService
{
    public async Task<string> GetChatResponse(string input)
    {
        var message = ChatMessage.CreateUserMessage(input);
        var response = await chatClient.CompleteChatAsync(message);
        return response.Value.Content.Last().Text;
    }
}
