namespace ApiService.Extensions;

public static class ConfigurationExtensions
{
    public static T GetRequiredEnum<T>(this IConfiguration configuration, string key) where T : struct, Enum
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value) || !Enum.TryParse<T>(value, true, out var result))
        {
            throw new InvalidOperationException($"Configuration key '{key}' is missing or invalid, found: '{value}'");
        }
        return result;
    }
}
