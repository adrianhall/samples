using CommunityToolkit.Datasync.Server.InMemory;
using System.ComponentModel.DataAnnotations;

namespace InMemoryDatasyncService.Models;

public class CategoryDTO : InMemoryTableData
{
    [Required, StringLength(64, MinimumLength = 1)]
    public string CategoryName { get; set; } = string.Empty;
}
