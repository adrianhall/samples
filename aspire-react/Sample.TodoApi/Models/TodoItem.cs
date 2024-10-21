using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sample.TodoApi.Models;

public class TodoItem : EntityTableData
{
    public string? UserId { get; set; }

    [Required, JsonPropertyName("listId")]
    public string? ListId { get; set; }

    [Required, JsonPropertyName("name")]
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string State { get; set; } = "todo";

    public DateTimeOffset? DueDate { get; set; }

    public DateTimeOffset? CompletedDate { get; set; }
}
