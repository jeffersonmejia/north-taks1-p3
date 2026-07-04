using Microsoft.AspNetCore.Authentication;
using NorthwindStore.Helpers;
using NorthwindStore.Interfaces;
using System.Security.Claims;

namespace NorthwindStore.Middlewares;

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
