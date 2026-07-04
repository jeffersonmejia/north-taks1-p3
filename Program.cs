using NorthwindStore.Infrastructure;
using Serilog;

try
{
    var app = WebApplication
        .CreateBuilder(args)
        .ConfigureNorthwindStore()
        .Build();

    await app.RunNorthwindStoreAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
