using NorthwindStore.Data;
using Microsoft.EntityFrameworkCore;

namespace NorthwindStore.Infrastructure;

public static class WebApplicationExtensions
{
    public static async Task EnsureDatabasesAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var identityDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await identityDb.Database.EnsureCreatedAsync();
            await IdentitySeeder.SeedAsync(scope.ServiceProvider);
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "Database initialization skipped — app will run in reduced mode");
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
