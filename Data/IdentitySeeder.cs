using Microsoft.AspNetCore.Identity;
using NorthwindStore.Infrastructure;
using NorthwindStore.Models.Identity;

namespace NorthwindStore.Data;

public static class IdentitySeeder
{
    public const string AdminEmail = "admin@northwind.local";
    private const string AdminPasswordHash = "AQAAAAIAAYagAAAAEGCfGeL1WxcLSnc9vD5Ak8u4Cdv1yCVSN37Bho6ru+WaVAsnqzJwqc1iSH+R8OoYBw==";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { RoleNames.Admin, RoleNames.Customer, RoleNames.Employee })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = await userManager.FindByEmailAsync(AdminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true,
                FullName = "Northwind Admin",
                PasswordHash = AdminPasswordHash,
                SecurityStamp = "3F0F9071-EDAE-44EC-8C25-C2AAEC644AA7"
            };
            var result = await userManager.CreateAsync(admin);
            if (!result.Succeeded)
            {
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(admin, RoleNames.Admin))
        {
            await userManager.AddToRoleAsync(admin, RoleNames.Admin);
        }
    }
}
