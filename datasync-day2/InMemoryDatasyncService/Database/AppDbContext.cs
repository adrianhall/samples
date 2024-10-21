using InMemoryDatasyncService.Models;
using Microsoft.EntityFrameworkCore;

namespace InMemoryDatasyncService.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<Category> Categories => Set<Category>();
}
