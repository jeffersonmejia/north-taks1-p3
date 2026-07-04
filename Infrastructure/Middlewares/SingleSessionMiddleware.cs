using Microsoft.AspNetCore.Authentication;
using NorthwindStore.Infrastructure;
using NorthwindStore.Repositories;
using NorthwindStore.Services;
using System.Security.Claims;

namespace NorthwindStore.Infrastructure;

public class SingleSessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ISessionControlService sessions)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessionId = context.User.FindFirstValue(Constants.SessionClaimType);

            if (userId is null || !await sessions.IsSessionValidAsync(userId, sessionId))
            {
                await context.SignOutAsync();
                context.Response.Redirect("/Account/Login?expired=true");
                return;
            }
        }

        await next(context);
    }
}
