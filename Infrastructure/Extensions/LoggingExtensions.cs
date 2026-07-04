using Serilog;
using Serilog.Formatting.Compact;

namespace NorthwindStore.Infrastructure;

public static class LoggingExtensions
{
    public static WebApplicationBuilder ConfigureNorthwindStoreLogging(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .Enrich.FromLogContext();

        if (builder.Environment.IsDevelopment())
        {
            logger
                .MinimumLevel.Information()
                .WriteTo.File(new CompactJsonFormatter(), "logs/app-.json",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 10L * 1024 * 1024,
                retainedFileCountLimit: 7,
                rollOnFileSizeLimit: true);
        }

        Log.Logger = logger.CreateLogger();
        builder.Host.UseSerilog();

        return builder;
    }
}
