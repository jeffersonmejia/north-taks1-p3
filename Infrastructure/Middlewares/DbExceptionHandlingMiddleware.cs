using System.Data.Common;

namespace NorthwindStore.Infrastructure;

public class DbExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DbExceptionHandlingMiddleware> _logger;

    public DbExceptionHandlingMiddleware(RequestDelegate next, ILogger<DbExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "Database operation failed");

            if (!context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.StatusCode = 503;
                context.Items["DbError"] = true;
                context.Items["StatusCode"] = 503;

                var originalPath = context.Request.Path;
                var originalQueryString = context.Request.QueryString;

                context.SetEndpoint(null);
                context.Request.RouteValues.Clear();
                context.Request.Path = "/Status/Unavailable";
                context.Request.QueryString = QueryString.Empty;

                try
                {
                    await _next(context);
                }
                finally
                {
                    context.Request.Path = originalPath;
                    context.Request.QueryString = originalQueryString;
                }
            }
        }
    }
}
