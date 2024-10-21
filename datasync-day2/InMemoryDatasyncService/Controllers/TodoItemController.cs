using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using InMemoryDatasyncService.Database;
using InMemoryDatasyncService.Models;
using Microsoft.AspNetCore.Mvc;

namespace InMemoryDatasyncService.Controllers;

[Route("tables/[controller]")]
public class TodoItemController : TableController<TodoItem>
{
    public TodoItemController(AppDbContext context, ILogger<TodoItemController> logger) : base()
    {
        Repository = new EntityTableRepository<TodoItem>(context);
        Logger = logger;
        Options = new()
        {
            DisableClientSideEvaluation = true,
            EnableSoftDelete = true,
            PageSize = 10
        };

    }
}
