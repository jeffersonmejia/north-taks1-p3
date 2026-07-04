namespace NorthwindStore.Infrastructure;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureNorthwindStore(this WebApplicationBuilder builder)
    {
        builder.ConfigureNorthwindStoreLogging();
        builder.Configuration.AddProjectSecrets();
        builder.Services.AddNorthwindStore(builder.Configuration);

        return builder;
    }
}
