using NorthwindStore.Infrastructure;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(new CompactJsonFormatter(), "logs/app-.json",
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 10L * 1024 * 1024,
        retainedFileCountLimit: 7,
        rollOnFileSizeLimit: true)
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Configuration.AddProjectSecrets();
    builder.Services.AddNorthwindStore(builder.Configuration);

    var app = builder.Build();

    app.UseNorthwindStorePipeline();

    await app.EnsureDatabasesAsync();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
