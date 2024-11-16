using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Server.Models;

public class TodoItem : EntityTableData
{
    [Required, StringLength(128, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;
    public bool Completed { get; set; }
}
