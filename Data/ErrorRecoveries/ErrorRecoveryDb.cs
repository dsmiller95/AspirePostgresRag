namespace Data.ErrorRecoveries;

public class ErrorRecoveryDb
{
    public int Id { get; init; }
    public required string NormalizationKey { get; init; }
    public required string ErrorContent { get; init; }
    public required string ErrorResponse { get; init; } 
}
