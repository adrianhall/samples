using Microsoft.AspNetCore.Identity;
using Samples.Identity.Data;

namespace Samples.Identity.Extensions;

public static class StartupExtensions
{
    /// <summary>
    /// Adds the database into the services for this application.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    public static void AddDatabase(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<ApplicationDbContext>("identitydb", configureDbContextOptions: options =>
        {
            if (builder.Environment.IsDevelopment())
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
                options.EnableThreadSafetyChecks();
            }
        });

        builder.Services.AddScoped<IDbInitializer, ApplicationDbInitializer>();
    }

    /// <summary>
    /// Adds ASP.NET Identity into the services for this application.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    public static void AddAspNetIdentity(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

                options.SignIn.RequireConfirmedAccount = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();
    }

    /// <summary>
    /// Initializes the database.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>A task that resolves when complete.</returns>
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IDbInitializer initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await initializer.InitializeDatabaseAsync();
    }
}
