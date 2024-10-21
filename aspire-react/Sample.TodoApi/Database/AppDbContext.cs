using Microsoft.EntityFrameworkCore;
using Sample.TodoApi.Models;

namespace Sample.TodoApi.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoList> Lists => Set<TodoList>();

    public DbSet<TodoItem> Items => Set<TodoItem>();

    public async Task InitializeAsync(IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            await Database.EnsureDeletedAsync();
            await Database.EnsureCreatedAsync();
        }
    }
}
