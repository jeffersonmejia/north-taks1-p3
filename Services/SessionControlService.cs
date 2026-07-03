using Microsoft.AspNetCore.Identity;
using NorthwindStore.Interfaces;
using NorthwindStore.Models.Identity;

namespace NorthwindStore.Services;

public class SessionControlService(UserManager<ApplicationUser> users) : ISessionControlService
{
    public async Task<string> StartSessionAsync(ApplicationUser user)
    {
        user.ActiveSessionId = Guid.NewGuid().ToString("N");
        await users.UpdateAsync(user);
        return user.ActiveSessionId;
    }

    public async Task<bool> IsSessionValidAsync(string userId, string? sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return false;
        }

        var user = await users.FindByIdAsync(userId);
        return user is not null && user.ActiveSessionId == sessionId;
    }

    public async Task EndSessionAsync(ApplicationUser user)
    {
        user.ActiveSessionId = null;
        await users.UpdateAsync(user);
    }
}
