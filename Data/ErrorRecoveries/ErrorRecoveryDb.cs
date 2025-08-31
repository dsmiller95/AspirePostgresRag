namespace Data.ErrorRecoveries;

public class ErrorRecoveryDb
{
    public int Id { get; init; }
    public required bool Active { get; init; }
    public required string NormalizationKey { get; init; }
    public required string ErrorContent { get; init; }
    public required string ErrorContentSummary { get; init; }
    public required string ErrorResponse { get; init; } 
    public required int ErrorResponseStatusCode { get; init; } 
}
