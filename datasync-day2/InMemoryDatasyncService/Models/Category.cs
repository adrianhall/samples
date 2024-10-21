using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InMemoryDatasyncService.Models;

public class Category
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string MobileId { get; set; } = Guid.NewGuid().ToString("N");

    public DateTimeOffset? UpdatedAt { get; set; }

    [ConcurrencyCheck]
    public byte[] Version { get; set; } = Guid.NewGuid().ToByteArray();

    public string CategoryName { get; set; } = string.Empty;
}
