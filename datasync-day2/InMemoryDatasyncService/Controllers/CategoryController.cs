using CommunityToolkit.Datasync.Server;
using InMemoryDatasyncService.Models;
using Microsoft.AspNetCore.Mvc;

namespace InMemoryDatasyncService.Controllers;

[Route("tables/[controller]")]
public class CategoryController : TableController<CategoryDTO>
{
    public CategoryController(IRepository<CategoryDTO> repository, ILogger<CategoryController> logger)
        : base(repository)
    {
        Logger = logger;
    }
}
