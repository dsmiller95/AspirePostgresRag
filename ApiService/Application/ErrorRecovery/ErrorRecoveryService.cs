using System.Text.Json;
using System.Text.RegularExpressions;
using Data;
using Data.ErrorRecoveries;
using Domain.ErrorRecovery;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;

namespace ApiService.Application.ErrorRecovery;

public interface IErrorRecoveryService
{
    Task<ErrorRecoveryResult> GetRecovery(ErrorRecoveryRequest request);
}

public partial class ErrorRecoveryService(
    ChatClient chatClient,
    AppDbContext dbContext,
    ILogger<ErrorRecoveryService> logger) : IErrorRecoveryService
{
    public async Task<ErrorRecoveryResult> GetRecovery(ErrorRecoveryRequest request)
    {
        var summary = GetErrorContentSummary(request);
        var existing = await GetExistingErrorRecovery(request.NormalizationKey, summary);
        if (existing is not null)
        {
            return new ErrorRecoveryResult(existing.ErrorResponseStatusCode, existing.ErrorResponse);
        }
        
        var recovery = await GetRecovery(summary);
        var dbEntry = new ErrorRecoveryDb
        {
            NormalizationKey = request.NormalizationKey,
            ErrorContent = summary,
            ErrorResponse = recovery.ResultJson,
            ErrorResponseStatusCode = recovery.Status
        };
        dbContext.ErrorRecoveries.Add(dbEntry);
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to save error recovery to database");
            throw;
        }
        
        return recovery;
    }

    private async Task<ErrorRecoveryResult> GetRecovery(string contentSummary)
    {
        logger.LogInformation("Error summary:\n{Summary}", contentSummary);
        
        var response = await chatClient.CompleteChatAsync(
            ChatMessage.CreateSystemMessage("You are an expert software engineer who helps to recover from errors in code."),
            ChatMessage.CreateUserMessage(
@"""
Given the following error information, provide a response in json format which represents a consistent error response
object, usable by our downstream clients. The response object should describe the error to clients of the api:
if it is something they can fix then this should be clear. If it is something they cannot fix, this should also be clear.

Respond in this format. This can be any json object, and it will be returned to the client.
It must include a status key which is an integer HTTP status code.
```
{
  ""status"": 123,
  ""someKey"": ""someValue"",
  ""someObject"": {
    ""someNestedKey"": ""someNestedValue""
  }
}
```

Following is the Error Information:

--- --- --- ---
""" + contentSummary + @"""
--- --- --- ---

Provide a response in the specified format:
""")
            );

        var regex = ResponseParseRegex();
        var responseContent = response.Value.Content.Last().Text;
        var regexMatch = regex.Match(responseContent);
        if(!regexMatch.Success)
            throw new Exception("Invalid response format: Regex match failed");
        
        var jsonStringFromRegex = regexMatch.Groups[0].Value;
        var statusCodeParsed = JsonSerializer.Deserialize<ResponseErrorCode>(jsonStringFromRegex);
        var statusCode = statusCodeParsed?.status ?? 500;

        return new ErrorRecoveryResult(statusCode, jsonStringFromRegex);
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

    private async Task<ErrorRecoveryDb?> GetExistingErrorRecovery(string normalizationKey, string errorContent)
    {
        return await dbContext.ErrorRecoveries
            .Where(x => x.NormalizationKey == normalizationKey)
            .Where(x => x.ErrorContent == errorContent)
            .FirstOrDefaultAsync();
    }
    
    // ReSharper disable InconsistentNaming
    private record ResponseErrorCode(int? status);
    // ReSharper restore InconsistentNaming
    
    [GeneratedRegex(@"{(?:.|\s)+}")]
    private static partial Regex ResponseParseRegex();
}
