using CommunityToolkit.Datasync.Server;

namespace DatasyncService.Models;

public class TodoItemDTO : ITableData
{
    public string Id { get; set; } = string.Empty;
    public bool Deleted { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public byte[] Version { get; set; } = [];
    public string Text { get; set; } = string.Empty;
    public bool IsComplete { get; set; }

    public bool Equals(ITableData? other)
        => other is not null && Id == other.Id && Version.SequenceEqual(other.Version);
}
