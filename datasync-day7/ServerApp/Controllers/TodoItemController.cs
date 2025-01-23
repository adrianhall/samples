using CommunityToolkit.Datasync.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;

namespace ServerApp.Controllers;

[Authorize]
[Route("tables/todoitem")]
public class TodoItemController(IRepository<TodoItem> repository) : TableController<TodoItem>(repository)
{
}
