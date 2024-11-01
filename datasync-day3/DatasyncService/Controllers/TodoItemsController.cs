using CommunityToolkit.Datasync.Server;
using DatasyncService.Database;
using DatasyncService.Datasync;
using DatasyncService.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatasyncService.Controllers;

[Route("tables/todoitems")]
public class TodoItemsController : TableController<TodoItemDTO>
{
    public TodoItemsController(AppDbContext context) : base()
    {
        Repository = new TodoItemRepository(context);
    }
}
