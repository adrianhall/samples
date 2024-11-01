using DatasyncService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace DatasyncService.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>()
            .ToTable(tb => tb.HasTrigger("trg_TodoItems_UpdatedAt"));

        modelBuilder.Entity<TodoItem>().Property(p => p.CreatedAt)
            .HasDefaultValueSql("getdate()");

        modelBuilder.Entity<TodoItem>().Property(p => p.UpdatedAt)
            .ValueGeneratedOnAddOrUpdate()
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);

        // Additions for the Datasync Community Toolkit
        modelBuilder.Entity<TodoItem>().Property(p => p.MobileId)
            .ValueGeneratedOnAdd()
            .HasValueGenerator(typeof(SequentialGuidValueGenerator));

        modelBuilder.Entity<TodoItem>().HasIndex(p => p.MobileId).IsUnique();
        modelBuilder.Entity<TodoItem>().HasIndex(p => p.UpdatedAt);
        modelBuilder.Entity<TodoItem>().HasIndex(p => p.Deleted);
    }
}
