using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NorthwindStore.Data;
using NorthwindStore.Filters;
using NorthwindStore.Interfaces;
using NorthwindStore.Models.Identity;
using NorthwindStore.Repositories;
using NorthwindStore.Services;

namespace NorthwindStore.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNorthwindStore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddIdentitySecurity();
        services.AddWebInfrastructure();
        services.AddApplicationServices();
        services.AddControllersWithViews();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var northwindConnection = configuration.GetRequiredConnectionString("NorthwindConnection");
        var identityConnection = configuration.GetRequiredConnectionString("IdentityConnection");

        services.AddDbContext<NorthwindContext>(options => options.UseNpgsql(northwindConnection));
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(identityConnection));

        return services;
    }

    private static IServiceCollection AddIdentitySecurity(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(2);
            options.SlidingExpiration = true;
        });

        return services;
    }

    private static IServiceCollection AddWebInfrastructure(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.Name = ".NorthwindStore.Session";
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ValidateCartFilter>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<ISessionControlService, SessionControlService>();

        return services;
    }
}
