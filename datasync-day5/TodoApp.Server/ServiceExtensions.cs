using TodoApp.Server.Database;

namespace TodoApp.Server;

public static class ServiceExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app, TimeSpan timeout)
    {
        using CancellationTokenSource cts = new(timeout);
        using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.InitializeDatabaseAsync(cts.Token);
    }
}
