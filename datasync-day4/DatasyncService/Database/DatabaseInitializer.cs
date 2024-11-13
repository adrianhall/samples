namespace DatasyncService.Database;

public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

public static class DatabaseExtensions
{
    public static Task InitializeDatabaseAsync(this WebApplication app, TimeSpan timeout)
    {
        using CancellationTokenSource cts = new(timeout);
        using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        IDatabaseInitializer initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        return initializer.InitializeAsync(cts.Token);
    }
}

public class DatabaseInitializer(AppDbContext context, IWebHostEnvironment env, ILogger<DatabaseInitializer> logger) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("DbInitializer - env = {env}", env.EnvironmentName);
        if (env.IsDevelopment())
        {
            logger.LogWarning("Development mode: deleting database");
            bool deleted = await context.Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
            if (deleted)
            {
                logger.LogWarning("Development mode: database deleted using EnsureDeletedAsync");
            }
        }

        logger.LogInformation("Ensuring database is created.");
        bool created = await context.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
        if (created)
        {
            logger.LogInformation("Database created using EnsureCreatedAsync");
        }
    }
}
