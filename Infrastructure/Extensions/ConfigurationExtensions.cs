namespace NorthwindStore.Infrastructure;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddProjectSecrets(this IConfigurationBuilder configuration)
    {
        return configuration.AddJsonFile("Secrets/secrets.json", optional: false, reloadOnChange: true);
    }

    public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
    {
        return configuration.GetConnectionString(name)
            ?? throw new InvalidOperationException($"{name} is missing. Configure it in Secrets/secrets.json.");
    }
}
