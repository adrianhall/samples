using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using TodoApp.Server.Database;
using TodoApp.Server.Models;

namespace TodoApp.Server.Controllers;

[Route("tables/todoitems")]
public class TodoItemsController : TableController<TodoItem>
{
    public TodoItemsController(AppDbContext context) : base()
    {
        Repository = new EntityTableRepository<TodoItem>(context);
    }
}
