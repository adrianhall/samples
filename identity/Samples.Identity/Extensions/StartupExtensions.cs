using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
using Microsoft.AspNetCore.Identity;
using Samples.Identity.Data;
using Samples.Identity.Models;
using Samples.Identity.Services;

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
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var identityBuilder = builder.Services.AddAuthentication();
        IConfigurationSection fbCfg = builder.Configuration.GetSection("Identity:Facebook");
        if (fbCfg.HasKey("ClientId") && fbCfg.HasKey("ClientSecret"))
        {
            identityBuilder.AddFacebook(options =>
            {
                options.ClientId = fbCfg.GetRequiredString("ClientId");
                options.ClientSecret = fbCfg.GetRequiredString("ClientSecret");
            });
        }

        IConfigurationSection googleCfg = builder.Configuration.GetSection("Identity:Google");
        if (googleCfg.HasKey("ClientId") && googleCfg.HasKey("ClientSecret"))
        {
            identityBuilder.AddGoogle(options =>
            {
                options.ClientId = googleCfg.GetRequiredString("ClientId");
                options.ClientSecret = googleCfg.GetRequiredString("ClientSecret");
            });
        }

        IConfigurationSection msftCfg = builder.Configuration.GetSection("Identity:MicrosoftAccount");
        if (msftCfg.HasKey("ClientId") && msftCfg.HasKey("ClientSecret"))
        {
            identityBuilder.AddMicrosoftAccount(options =>
            {
                options.ClientId = msftCfg.GetRequiredString("ClientId");
                options.ClientSecret = msftCfg.GetRequiredString("ClientSecret");
            });
        }

        IConfiguration linkedInCfg = builder.Configuration.GetSection("Identity:LinkedIn");
        if (linkedInCfg.HasKey("ClientId") && linkedInCfg.HasKey("ClientSecret"))
        {
            identityBuilder.AddLinkedIn(options =>
            {
                options.ClientId = linkedInCfg.GetRequiredString("ClientId");
                options.ClientSecret = linkedInCfg.GetRequiredString("ClientSecret");
            });
        }

        builder.Services.Configure<AccountControllerOptions>(builder.Configuration.GetSection("Identity:Options"));

        builder.Services.Configure<MailerSendOptions>(builder.Configuration.GetSection("Services:MailerSend"));
        builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
        builder.Services.AddScoped<ISendEmailApi, MailerSendApi>();
        builder.Services.AddScoped<IEmailSender<ApplicationUser>, MailerSendEmailSender>();
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
            pipeline.AddScssBundle("/css/site.css", "/css/site.scss");

            if (!builder.Environment.IsDevelopment())
            {
                pipeline.MinifyCssFiles();
                pipeline.AddFiles("text/css", "/css/*");

                pipeline.MinifyJsFiles();
                pipeline.AddFiles("text/javascript", "/js/*");
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

    /// <summary>
    /// Adds ASP.NET Identity into the HTTP pipeline for this application.
    /// </summary>
    /// <param name="builder">The web application to modify.</param>
    internal static void UseAspNetIdentity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
