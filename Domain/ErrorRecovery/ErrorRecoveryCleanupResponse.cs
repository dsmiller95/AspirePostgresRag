namespace Domain.ErrorRecovery;

public record ErrorRecoveryCleanupResponse(List<ErrorRecoveryCleanupResponseDetail> Details, int TotalStaleRecoveries);

public record ErrorRecoveryCleanupResponseDetail(string NormalizationKey, string JsonSchema, string? PreviousJsonSchema);
