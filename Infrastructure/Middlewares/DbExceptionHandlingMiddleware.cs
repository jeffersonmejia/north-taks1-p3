using System.Data.Common;
using Npgsql;

namespace NorthwindStore.Infrastructure;

public class DbExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DbExceptionHandlingMiddleware> _logger;

    private const string ReEntryKey = "DbExceptionHandled";

    public DbExceptionHandlingMiddleware(RequestDelegate next, ILogger<DbExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Items.ContainsKey(ReEntryKey))
        {
            await _next(context);
            return;
        }

        try
        {
            await _next(context);
        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "Database operation failed — path: {Path}", context.Request.Path);

            if (!context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.StatusCode = 503;

                context.Items["DbError"] = true;
                context.Items["StatusCode"] = 503;
                context.Items[ReEntryKey] = true;

                var originalPath = context.Request.Path;
                context.Request.Path = "/Status/Unavailable";

                try
                {
                    await _next(context);
                }
                catch (Exception fallbackEx)
                {
                    _logger.LogCritical(fallbackEx, "Error page also failed — writing raw response");
                    context.Response.StatusCode = 503;
                    await context.Response.WriteAsync("Service temporarily unavailable — database connection failed.");
                }
                finally
                {
                    context.Request.Path = originalPath;
                }
            }
        }
    }
}
