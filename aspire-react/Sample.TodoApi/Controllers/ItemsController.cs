using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Sample.TodoApi.Database;
using Sample.TodoApi.Models;

namespace Sample.TodoApi.Controllers;

[Route("tables/[controller]")]
public class ItemsController : TableController<TodoItem>
{
    public ItemsController(AppDbContext context, ILogger<ItemsController> logger)
    {
        Repository = new EntityTableRepository<TodoItem>(context);
        Options = new TableControllerOptions { EnableSoftDelete = true };
        Logger = logger;
    }
}