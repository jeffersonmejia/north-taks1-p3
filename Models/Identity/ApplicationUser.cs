using Microsoft.AspNetCore.Identity;

namespace NorthwindStore.Models.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? CustomerId { get; set; }
    public string? ActiveSessionId { get; set; }
}
