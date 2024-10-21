using InMemoryDatasyncService.Models;

namespace InMemoryDatasyncService.Database;

public interface IDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

public class DbInitializer(AppDbContext context, IWebHostEnvironment env, ILogger<DbInitializer> logger) : IDbInitializer
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

        if (!context.TodoItems.Any())
        {
            logger.LogInformation("Adding seed data to TodoItems");
            await InitializeTodoItems(cancellationToken).ConfigureAwait(false);
        }

        if (!context.Categories.Any())
        {
            logger.LogInformation("Adding seed data to Categories");
            await InitializeCategories(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task InitializeTodoItems(CancellationToken cancellationToken = default)
    {
        string[] seed = [
            "Learn about Datasync",
            "Learn MAUI",
            "Buy Milk",
            "Read apps-on-azure-blog"
        ];

        int i = 0;
        foreach (string title in seed)
        {
            i--;
            TodoItem item = new()
            {
                Id = Guid.NewGuid().ToString("N"),
                UpdatedAt = DateTimeOffset.UtcNow.AddDays(i),
                Version = Guid.NewGuid().ToByteArray(),
                Deleted = false,
                Title = title,
                IsComplete = false
            };
            context.TodoItems.Add(item);
        }
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task InitializeCategories(CancellationToken cancellationToken = default)
    {
        string[] seed = [
            "learning",
            "shopping"
        ];

        int i = 0;
        foreach (string categoryName in seed)
        {
            i--;
            Category item = new()
            {
                UpdatedAt = DateTimeOffset.UtcNow.AddDays(i),
                CategoryName = categoryName
            };
            context.Categories.Add(item);
        }
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
