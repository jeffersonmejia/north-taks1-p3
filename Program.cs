using NorthwindStore.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log",
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
