using NorthwindStore.Data;
using NorthwindStore.Infrastructure;

namespace NorthwindStore.Infrastructure;

public static class WebApplicationExtensions
{
    public static async Task SeedIdentityAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            await IdentitySeeder.SeedAsync(scope.ServiceProvider);
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "Identity seeding skipped — database may be unavailable");
        }
    }

    public static WebApplication UseNorthwindStorePipeline(this WebApplication app)
    {
        app.UseExceptionHandler("/Status/Error");
        app.UseStatusCodePagesWithReExecute("/Status/Error");

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseMiddleware<DbExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
        app.UseMiddleware<SingleSessionMiddleware>();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Products}/{action=Index}/{id?}");

        return app;
    }
}
