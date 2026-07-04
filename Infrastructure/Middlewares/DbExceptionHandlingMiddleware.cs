using System.Data.Common;
using System.Text;

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
                context.Response.ContentType = "text/html; charset=utf-8";

                var html = """
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="utf-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1">
                    <title>Loading... - NorthwindStore</title>
                    <style>
                        * { margin: 0; padding: 0; box-sizing: border-box; }
                        body {
                            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                            background: #f5f5f5;
                            color: #333;
                            display: flex;
                            align-items: center;
                            justify-content: center;
                            min-height: 100vh;
                            padding: 20px;
                        }
                        .card {
                            background: #fff;
                            border: 1px solid #e0e0e0;
                            border-radius: 16px;
                            padding: 40px;
                            max-width: 480px;
                            text-align: center;
                            box-shadow: 0 2px 12px rgba(0,0,0,0.06);
                        }
                        .spinner {
                            width: 40px;
                            height: 40px;
                            border: 3px solid #e0e0e0;
                            border-top-color: #6366f1;
                            border-radius: 50%;
                            animation: spin 0.8s linear infinite;
                            margin: 0 auto 20px;
                        }
                        @keyframes spin { to { transform: rotate(360deg); } }
                        .card p { color: #555; line-height: 1.6; }
                    </style>
                </head>
                <body>
                    <div class="card">
                        <div class="spinner"></div>
                        <p><strong>Loading...</strong></p>
                        <p style="margin-top:12px;font-size:0.9rem;color:#666;">The service is temporarily unavailable. Please try again later.</p>
                    </div>
                    <script>
                        (function retry() {
                            fetch(location.href)
                                .then(r => { if (r.ok) location.reload(); })
                                .catch(() => {});
                            setTimeout(retry, 3000);
                        })();
                    </script>
                </body>
                </html>
                """;

                await context.Response.WriteAsync(html, Encoding.UTF8);
            }
        }
    }
}
