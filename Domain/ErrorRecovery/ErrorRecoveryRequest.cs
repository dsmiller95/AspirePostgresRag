namespace Domain.ErrorRecovery;

public record ErrorRecoveryRequest(string NormalizationKey, Exception Exception, string Context) : IHaveExample {
    
    public static object GetExample() => new ErrorRecoveryRequest(
        NormalizationKey: "ExampleNormalizationKey",
        Exception: new Exception("Example exception message"),
        Context: "Example context information"
    );
}
