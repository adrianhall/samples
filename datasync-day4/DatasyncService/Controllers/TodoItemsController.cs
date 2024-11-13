using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using DatasyncService.Database;
using DatasyncService.Datasync;
using DatasyncService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatasyncService.Controllers;

[Authorize, Route("tables/todoitems")]
public class TodoItemsController : TableController<TodoItem>
{
    public TodoItemsController(AppDbContext context, IHttpContextAccessor contextAccessor, ILogger<TodoItemsController> logger) : base()
    {
        Repository = new EntityTableRepository<TodoItem>(context);
        AccessControlProvider = new PersonalAccessControlProvider<TodoItem>(contextAccessor);
        Logger = logger;
    }
}
