namespace Data.ErrorRecoveries;

public class ErrorRecoverySchemaDb
{
    public int Id { get; init; }
    public required string NormalizationKey { get; init; }
    public required string JsonSchema { get; init; } 
}
