namespace Domain.ErrorRecovery;

public record ErrorRecoveryCleanupRequest() : IHaveExample {
    
    public static object GetExample() => new ErrorRecoveryCleanupRequest();
}
