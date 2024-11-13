using CommunityToolkit.Datasync.Server;
using DatasyncService.Models;
using System.Linq.Expressions;

namespace DatasyncService.Datasync;

public class PersonalAccessControlProvider<TEntity>(IHttpContextAccessor contextAccessor)
    : IAccessControlProvider<TEntity> where TEntity : ITableData, IPersonalEntity
{
    private string? UserId { get => contextAccessor.HttpContext?.User?.Identity?.Name; }

    public Expression<Func<TEntity, bool>> GetDataView()
       => UserId is null ? x => false : x => x.UserId == UserId;

    public ValueTask<bool> IsAuthorizedAsync(TableOperation op, TEntity? entity, CancellationToken cancellationToken = default)
      => ValueTask.FromResult(op is TableOperation.Create || op is TableOperation.Query || entity?.UserId == UserId);

    public ValueTask PreCommitHookAsync(TableOperation op, TEntity entity, CancellationToken cancellationToken = default)
    {
        if (UserId is null)
        {
            throw new HttpException(StatusCodes.Status401Unauthorized);
        }

        entity.UserId = UserId;
        return ValueTask.CompletedTask;
    }

    public ValueTask PostCommitHookAsync(TableOperation op, TEntity entity, CancellationToken cancellationToken = default)
      => ValueTask.CompletedTask;
}
