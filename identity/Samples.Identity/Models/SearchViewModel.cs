using System.ComponentModel.DataAnnotations;

namespace Samples.Identity.Models;

public record SearchViewModel
{
    [Required, MinLength(1), MaxLength(100), RegularExpression("^[a-zA-Z@-_. ]+$")]
    public string Search { get; set; } = string.Empty;
}
