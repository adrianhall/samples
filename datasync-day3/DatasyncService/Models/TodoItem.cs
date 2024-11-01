using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DatasyncService.Models;

public class TodoItem
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }

    [Required, StringLength(128, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
    public bool IsComplete { get; set; }

    // Additional fields needed by the Datasync Community Toolkit
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string MobileId { get; set; } = Guid.NewGuid().ToString("N");

    [Timestamp]
    public byte[] Version { get; set; } = [];

    public bool Deleted { get; set; } = false;
}
