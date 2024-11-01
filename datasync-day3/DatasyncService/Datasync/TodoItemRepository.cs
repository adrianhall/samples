using CommunityToolkit.Datasync.Server;
using DatasyncService.Database;
using DatasyncService.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;

namespace DatasyncService.Datasync;

public class TodoItemRepository(AppDbContext dbContext) : IRepository<TodoItemDTO>
{
    public static readonly Func<TodoItem, int, TodoItemDTO> ToDTO = (x, idx) => new TodoItemDTO
    {
        Id = x.MobileId,
        Deleted = x.Deleted,
        UpdatedAt = x.UpdatedAt,
        Version = [..x.Version],
        Text = x.Text,
        IsComplete = x.IsComplete
    };

    public ValueTask<IQueryable<TodoItemDTO>> AsQueryableAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(dbContext.TodoItems.AsNoTracking().Select(ToDTO).AsQueryable());

    public async ValueTask CreateAsync(TodoItemDTO entity, CancellationToken cancellationToken = default)
    {
        // Step 1: Create the entity from the DTO
        TodoItem dbEntity = new()
        {
            // Don't set Id in this case.  It's set by the database
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Text = entity.Text,
            IsComplete = entity.IsComplete,
            Version = Guid.NewGuid().ToByteArray(),
            MobileId = string.IsNullOrEmpty(entity.Id) ? Guid.NewGuid().ToString("N") : entity.Id,
            Deleted = entity.Deleted
        };

        await WrapExceptionAsync(dbEntity.MobileId, async () =>
        {
            if (dbContext.TodoItems.Any(x => x.MobileId == dbEntity.MobileId))
            {
                throw new HttpException((int)HttpStatusCode.Conflict)
                {
                    Payload = await GetEntityAsync(dbEntity.MobileId, cancellationToken).ConfigureAwait(false)
                };
            }

            _ = dbContext.TodoItems.Add(dbEntity);
            _ = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            CopyBack(dbEntity, entity);
        }, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DeleteAsync(string id, byte[]? version = null, CancellationToken cancellationToken = default)
    {
        ThrowIfNullOrEmpty(id);
        await WrapExceptionAsync(id, async () =>
        {
            TodoItem storedEntity = await dbContext.TodoItems.SingleOrDefaultAsync(x => x.MobileId == id, cancellationToken).ConfigureAwait(false) 
                ?? throw new HttpException((int)HttpStatusCode.NotFound);
            if (version?.Length > 0 && !storedEntity.Version.SequenceEqual(version))
            {
                throw new HttpException((int)HttpStatusCode.PreconditionFailed)
                {
                    Payload = await GetEntityAsync(id, cancellationToken).ConfigureAwait(false)
                };
            }
            _ = dbContext.TodoItems.Remove(storedEntity);
            _ = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<TodoItemDTO> ReadAsync(string id, CancellationToken cancellationToken = default)
    {
        ThrowIfNullOrEmpty(id);
        TodoItem storedEntity = await dbContext.TodoItems.SingleOrDefaultAsync(x => x.MobileId == id, cancellationToken).ConfigureAwait(false)
            ?? throw new HttpException((int)HttpStatusCode.NotFound);
        return ToDTO(storedEntity, 0);
    }

    public async ValueTask ReplaceAsync(TodoItemDTO entity, byte[]? version = null, CancellationToken cancellationToken = default)
    {
        ThrowIfNullOrEmpty(entity.Id);
        await WrapExceptionAsync(entity.Id, async () =>
        {
            TodoItem storedEntity = await dbContext.TodoItems.SingleOrDefaultAsync(x => x.MobileId == entity.Id, cancellationToken).ConfigureAwait(false)
                ?? throw new HttpException((int)HttpStatusCode.NotFound);
            if (version?.Length > 0 && !storedEntity.Version.SequenceEqual(version))
            {
                throw new HttpException((int)HttpStatusCode.PreconditionFailed) 
                { 
                    Payload = await GetEntityAsync(entity.Id, cancellationToken).ConfigureAwait(false) 
                };
            }

            storedEntity.Text = entity.Text;
            storedEntity.IsComplete = entity.IsComplete;
            storedEntity.Deleted = entity.Deleted;
            // TODO: If your stored entity does not update UpdatedAt/Version, then do it here.
            dbContext.Entry(storedEntity).CurrentValues.SetValues(entity);
            _ = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            CopyBack(storedEntity, entity);
        }, cancellationToken).ConfigureAwait(false);
    }

    internal static void CopyBack(TodoItem dbEntity, TodoItemDTO entity)
    {
        entity.Id = dbEntity.MobileId;
        entity.UpdatedAt = dbEntity.UpdatedAt;
        entity.Version = [..dbEntity.Version];
        entity.Deleted = dbEntity.Deleted;
        // Add any other properties that could change during save here.
    }

    internal async Task WrapExceptionAsync(string id, Func<Task> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await action.Invoke().ConfigureAwait(false);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new HttpException((int)HttpStatusCode.Conflict, ex.Message, ex) { Payload = await GetEntityAsync(id, cancellationToken).ConfigureAwait(false) };
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException(ex.Message, ex);
        }
    }

    internal async Task<TodoItemDTO> GetEntityAsync(string id, CancellationToken cancellationToken = default)
    {
        TodoItem todoItem = await dbContext.TodoItems.SingleAsync(x => x.MobileId == id, cancellationToken);
        return ToDTO(todoItem, 0);
    }

    internal void ThrowIfNullOrEmpty(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new HttpException((int)HttpStatusCode.BadRequest, "ID is required");
        }
    }
}
