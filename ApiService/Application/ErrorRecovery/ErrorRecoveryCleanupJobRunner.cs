using Data;
using Data.ErrorRecoveries;
using Domain.ErrorRecovery;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;

namespace ApiService.Application.ErrorRecovery;

public interface IErrorRecoveryCleanupJobRunner
{
    Task<ErrorRecoveryCleanupResponse> CleanupJob(ErrorRecoveryCleanupRequest request);
}

public class ErrorRecoveryCleanupJobRunner(
    AppDbContext dbContext,
    ChatClient chatClient,
    ILogger<ErrorRecoveryCleanupJobRunner> logger
    ) : IErrorRecoveryCleanupJobRunner
{
    private readonly Random _random = new();
    public async Task<ErrorRecoveryCleanupResponse> CleanupJob(ErrorRecoveryCleanupRequest request)
    {
        var uniqueNormalizationKeys = await dbContext.ErrorRecoveries
            .Where(x => x.Active)
            .Select(x => x.NormalizationKey)
            .Distinct().ToListAsync();
        
        var pickedKeys = uniqueNormalizationKeys
            .OrderBy(x => _random.Next())
            .Take(5)
            .ToList();
        
        var updatedSchemas = new List<ErrorRecoveryCleanupResponseDetail>();
        var removedRecoveriesCount = 0;

        foreach (var uniqueNormalizationKey in pickedKeys)
        {
            var allSummaries = await dbContext.ErrorRecoveries
                .Where(x => x.Active)
                .Where(x => x.NormalizationKey == uniqueNormalizationKey)
                .Select(x => x.ErrorContentSummary)
                .ToListAsync();
            
            var existingSchema = await dbContext.ErrorRecoverySchemas
                .Where(x => x.NormalizationKey == uniqueNormalizationKey)
                .FirstOrDefaultAsync();
            
            var schema = await GetSchemaFromSummaries(allSummaries, existingSchema?.JsonSchema);
            updatedSchemas.Add(new ErrorRecoveryCleanupResponseDetail(uniqueNormalizationKey, schema, existingSchema?.JsonSchema));
            if (schema == existingSchema?.JsonSchema)
            {
                logger.LogWarning("Schema for {NormalizationKey} is unchanged", uniqueNormalizationKey);
                continue;
            }
            
            if (existingSchema is null)
            {
                var dbEntry = new ErrorRecoverySchemaDb
                {
                    NormalizationKey = uniqueNormalizationKey,
                    JsonSchema = schema
                };
                dbContext.ErrorRecoverySchemas.Add(dbEntry);
            }
            else
            {
                var newEntry = new ErrorRecoverySchemaDb()
                {
                    Id = existingSchema.Id, 
                    NormalizationKey = existingSchema.NormalizationKey,
                    JsonSchema = schema,
                };
                dbContext.ErrorRecoverySchemas.Update(newEntry);
            }

            var deleted = await dbContext.ErrorRecoveries
                .Where(x => x.NormalizationKey == uniqueNormalizationKey)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.Active, false));
            removedRecoveriesCount += deleted;

            await dbContext.SaveChangesAsync();
        }

        return new ErrorRecoveryCleanupResponse(updatedSchemas, removedRecoveriesCount);
    }

    private async Task<string> GetSchemaFromSummaries(List<string> summaries, string? existingSchema)
    {
        var response = await chatClient.CompleteChatAsync(
            ChatMessage.CreateSystemMessage(
                "You are an expert software engineer who designs comprehensive json schemas which will consistently represent a given summary of errors."),
            ChatMessage.CreateUserMessage(
                @"""
Given the following types of errors, create a comprehensive json schema which will consistently represent all of them.
This json schema must be capable of representing the errors, and must be as strict as possible while still being able to represent the errors.
Do not include any example values, only the schema.

Following is the current schema, if any:
--- --- --- --- 
""" + (existingSchema ?? "No existing schema") + @"
--- --- --- ---

Following are all the types of errors which must be represented by the schema.
--- --- --- ---
""" + string.Join("\n---\n",summaries) + @"""
--- --- --- ---
Provide only the json schema as the response, do not include any other text.
"""));

        return response.Value.Content.Last().Text;
    }
}
