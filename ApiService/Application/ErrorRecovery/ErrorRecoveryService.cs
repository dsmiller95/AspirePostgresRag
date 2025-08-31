using System.Text.Json;
using System.Text.RegularExpressions;
using ApiService.Application.Ai;
using Domain.ErrorRecovery;
using OpenAI.Chat;

namespace ApiService.Application.ErrorRecovery;

public interface IErrorRecoveryService
{
    Task<ErrorRecoveryResult> GetRecovery(ErrorRecoveryRequest request);
}

public partial class ErrorRecoveryService(ChatClient chatClient) : IErrorRecoveryService
{
    public async Task<ErrorRecoveryResult> GetRecovery(ErrorRecoveryRequest request)
    {
        var summary = GetErrorContentSummary(request);
        return new ErrorRecoveryResult(418, JsonSerializer.Serialize(new
        {
            Error = "shits fucked",
            Key = request.NormalizationKey,
            Context = request.Context,
            Summary = summary,
            ExceptionType = request.Exception.GetType().FullName
        }));
        
        
        var response = await chatClient.CompleteChatAsync(
            ChatMessage.CreateSystemMessage("You are an expert software engineer who helps to recover from errors in code."),
            ChatMessage.CreateUserMessage(
@"""
Given the following error information, provide a response in json format which represents a consistent error response
object, usable by our downstream clients. The response object should describe the error to clients of the api:
if it is something they can fix then this should be clear. If it is something they cannot fix, this should also be clear.

Respond in this format. this can be any json object, and it will be returned to the client. status is an integer HTTP status code.
```
{
  ""someKey"": ""someValue"",
  ""someObject"": {
    ""someNestedKey"": ""someNestedValue""
  }
}
Status: 123
```

Following is the Error Information:

--- --- --- ---
""" + summary + @"""
--- --- --- ---

Provide a response in the specified format:
""")
            );

        var regex = ResponseParseRegex();
        var responseContent = response.Value.Content.Last().Text;
        var regexMatch = regex.Match(responseContent);
        if(!regexMatch.Success)
            throw new Exception("Invalid response format: Regex match failed");
        
        var jsonStringFromRegex = regexMatch.Success ? regexMatch.Groups[1].Value : null;
        var statusStringFromRegex = regexMatch.Success ? regexMatch.Groups[2].Value : null;
        if (string.IsNullOrEmpty(jsonStringFromRegex) || string.IsNullOrEmpty(statusStringFromRegex) || !int.TryParse(statusStringFromRegex, out var statusFromRegex))
        {
            throw new Exception("Invalid response format: Unable to extract JSON or status code");
        }

        return new ErrorRecoveryResult(statusFromRegex, jsonStringFromRegex);
    }
    
    private string GetErrorContentSummary(ErrorRecoveryRequest request)
    {
        return $"""
        Normalization Key: {request.NormalizationKey}
        Exception Message: {request.Exception.Message}
        Context: {request.Context}
        Stack Trace: {request.Exception.StackTrace}
        """;
    }

    [GeneratedRegex(@"({(?:.|\s)+})\s*Status: (\d+)")]
    private static partial Regex ResponseParseRegex();
}
