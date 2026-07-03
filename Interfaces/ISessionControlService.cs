using NorthwindStore.Models.Identity;

namespace NorthwindStore.Interfaces;

public interface ISessionControlService
{
    Task<string> StartSessionAsync(ApplicationUser user);
    Task<bool> IsSessionValidAsync(string userId, string? sessionId);
    Task EndSessionAsync(ApplicationUser user);
}
