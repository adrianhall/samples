using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TodoApp.Service.Database;

namespace TodoApp.Service.Controllers;

[Route("tables/todoitems")]
public class TodoItemsController : TableController<TodoItem>
{
    public TodoItemsController(
        TodoContext context,
        IHubContext<ServiceHub> hubContext
        ) : base()
    {
        Repository = new EntityTableRepository<TodoItem>(context);
        RepositoryUpdated += async (sender, eventArgs) => await hubContext.Clients.All.SendAsync("ServiceChange", eventArgs);
    }
}


public class EventingProvider<T>(
    IHubContext<ServiceHub> hubContext
    ) : AccessControlProvider<T>() where T : EntityTableData
{
    public override async ValueTask PostCommitHookAsync(TableOperation operation, T entity, CancellationToken cancellationToken = default)
    {
        RepositoryUpdatedEventArgs eventArgs = new(operation, typeof(T).Name, entity);
        await hubContext.Clients.All.SendAsync("ServiceChange", eventArgs, cancellationToken);
    }
}