using DatasyncService.Models;
using Microsoft.EntityFrameworkCore;

namespace DatasyncService.Database;

public interface IDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

public class DbInitializer(
    AppDbContext context,
    IWebHostEnvironment env,
    ILogger<DbInitializer> logger
    ) : IDbInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("InitializeAsync: env={env}", env.EnvironmentName);
        if (env.IsDevelopment())
        {
            logger.LogWarning("Development mode: deleting database");
            bool deleted = await context.Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
            if (deleted)
            {
                logger.LogWarning("Development mode: database deleted using EnsureDeletedAsync");
            }
        }

        logger.LogInformation("InitializeAsync: ensuring database is created.");
        bool created = await context.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
        if (created)
        {
            logger.LogInformation("Database created using EnsureCreatedAsync");
            await InstallTriggersAsync(cancellationToken).ConfigureAwait(false);
        }

        if (!context.TodoItems.Any())
        {
            logger.LogInformation("Adding seed data to TodoItems");
            await InitializeTodoItemsAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task InitializeTodoItemsAsync(CancellationToken cancellationToken = default)
    {
        string[] seed = [
            "Learn about Datasync",
            "Learn MAUI",
            "Buy Milk",
            "Read apps-on-azure-blog"
        ];

        foreach (string title in seed)
        {
            TodoItem item = new() { Text = title };
            context.TodoItems.Add(item);
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task InstallTriggersAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            CREATE TRIGGER [dbo].[trg_TodoItems_UpdatedAt] ON [dbo].[TodoItems]
            AFTER INSERT, UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;
                UPDATE [dbo].[TodoItems] 
                SET [UpdatedAt] = SYSDATETIMEOFFSET()
                FROM [dbo].[TodoItems] t
                INNER JOIN inserted i ON t.Id = i.Id;
            END;
        ";

        await context.Database.ExecuteSqlRawAsync(sql, cancellationToken).ConfigureAwait(false);
    }
}
