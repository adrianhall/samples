using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DatasyncService.Models;

public class TodoItem : EntityTableData, IPersonalEntity
{
    [Required, StringLength(128, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    public bool IsComplete { get; set; }

    /// <summary>
    /// From IPersonalEntity
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}
