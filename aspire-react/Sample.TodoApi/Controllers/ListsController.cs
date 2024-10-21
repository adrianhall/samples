using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Sample.TodoApi.Database;
using Sample.TodoApi.Models;

namespace Sample.TodoApi.Controllers;

[Route("tables/[controller]")]
public class ListsController : TableController<TodoList>
{
    public ListsController(AppDbContext context, ILogger<ListsController> logger)
    {
        Repository = new EntityTableRepository<TodoList>(context);
        Options = new TableControllerOptions { EnableSoftDelete = true };
        Logger = logger;
    }
}
