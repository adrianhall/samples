using AutoMapper;
using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.Automapper;
using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using InMemoryDatasyncService.Database;
using InMemoryDatasyncService.Models;
using Microsoft.AspNetCore.Mvc;

namespace InMemoryDatasyncService.Controllers;

[Route("api/todoitem")]
public class AutomappedController : TableController<TodoItemDTO>
{
    public AutomappedController(IMapper mapper, AppDbContext context) : base()
    {
        var efrepo = new EntityTableRepository<TodoItem>(context);
        Repository = new MappedTableRepository<TodoItem, TodoItemDTO>(mapper, efrepo);
    }
}
