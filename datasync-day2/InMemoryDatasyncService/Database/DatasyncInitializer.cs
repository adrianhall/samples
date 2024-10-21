using CommunityToolkit.Datasync.Server;
using InMemoryDatasyncService.Models;
using Microsoft.EntityFrameworkCore;

namespace InMemoryDatasyncService.Database;

public interface IDatasyncInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

public class DatasyncInitializer(AppDbContext context, IRepository<CategoryDTO> repository) : IDatasyncInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        IList<Category> categories = await context.Categories.ToListAsync(cancellationToken).ConfigureAwait(false);
        IList<CategoryDTO> seed = categories.Select(Convert).ToList();

        IQueryable<CategoryDTO> queryable = await repository.AsQueryableAsync(cancellationToken);
        foreach (CategoryDTO dto in seed)
        {
            if (!queryable.Any(x => x.CategoryName.Equals(dto.CategoryName, StringComparison.OrdinalIgnoreCase)))
            {
                await repository.CreateAsync(dto, cancellationToken);
            }
        }
    }

    private static CategoryDTO Convert(Category category) => new()
    {
        Id = category.MobileId,
        UpdatedAt = category.UpdatedAt ?? DateTimeOffset.UnixEpoch,
        Version = category.Version ?? Guid.NewGuid().ToByteArray(),
        Deleted = false,
        CategoryName = category.CategoryName
    };
}
