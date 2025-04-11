namespace FileService.Common;

public static class ConfigurationExtensions
{
    public static T GetRequiredValue<T> (this IConfiguration configuration, string key)
    {
        var value = configuration.GetValue<T>(key);
        if (value == null)
        {
            throw new InvalidOperationException($"Configuration value for {typeof(T).Name} is required.");
        }
        return value;
    }
}
