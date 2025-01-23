using ClientApp.Interfaces;
using ClientApp.Services;
using ClientApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Desktop;
using System.Diagnostics;
using System.Windows;

namespace ClientApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, IDisposable
{
    public IServiceProvider Services { get; }

    public App()
    {
        Services = new ServiceCollection()
            .AddSingleton<IPublicClientApplication>((_) => CreateIdentityClient())
            .AddSingleton<ITodoService, OnlineTodoService>()
            .AddTransient<TodoListViewModel>()
            .AddScoped<IAlertService, AlertService>()
            .AddScoped<IAppInitializer, AppInitializer>()
            .BuildServiceProvider();

        InitializeApplication();
    }

    public IPublicClientApplication CreateIdentityClient()
    {
        var client = PublicClientApplicationBuilder.Create(Constants.ApplicationId)
            .WithAuthority(AzureCloudInstance.AzurePublic, "common")
            .WithRedirectUri("http://localhost")
            .WithWindowsEmbeddedBrowserSupport()
            .Build();
        Debug.WriteLine("IdentityClient created.");
        return client;
    }

    private void InitializeApplication()
    {
        using IServiceScope scope = Services.CreateScope();
        IAppInitializer initializer = scope.ServiceProvider.GetRequiredService<IAppInitializer>();
        initializer.Initialize();
    }

    /// <summary>
    /// A helper method for getting a service reference from dependency injection.
    /// </summary>
    public static TService GetRequiredService<TService>() where TService : notnull
        => ((App)App.Current).Services.GetRequiredService<TService>();

    #region IDisposable Support
    private bool hasDisposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!hasDisposed)
        {
            if (disposing)
            {
                // TODO: Dispose of any database connections
            }
            hasDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
