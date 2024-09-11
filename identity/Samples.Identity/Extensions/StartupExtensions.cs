using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
using Microsoft.AspNetCore.Identity;
using Samples.Identity.Data;

namespace Samples.Identity.Extensions;

internal static class StartupExtensions
{
    /// <summary>
    /// Adds the database into the services for this application.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    internal static void AddDatabase(this IHostApplicationBuilder builder)
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
    internal static void AddAspNetIdentity(this IHostApplicationBuilder builder)
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
    /// Adds LigerShark WebOptimizer configuration.
    /// </summary>
    /// <param name="builder"></param>
    internal static void AddWebOptimizer(this IHostApplicationBuilder builder)
    {
        builder.Services.AddJsEngineSwitcher(options =>
        {
            options.AllowCurrentProperty = false;
            options.DefaultEngineName = V8JsEngine.EngineName;
        }).AddV8();

        builder.Services.AddWebOptimizer(pipeline =>
        {
            pipeline.AddScssBundle("/css/styles.css", "/css/styles.scss");

            if (!builder.Environment.IsDevelopment())
            {
                pipeline.MinifyCssFiles();
                pipeline.AddFiles("text/css", "/css/*");
            }
        });
    }

    /// <summary>
    /// Initializes the database.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>A task that resolves when complete.</returns>
    internal static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IDbInitializer initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await initializer.InitializeDatabaseAsync();
    }
}
