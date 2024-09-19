using System.ComponentModel.DataAnnotations;

namespace Samples.Identity.Models.Account;

public class ExternalLoginErrorViewModel
{
    [Required, StringLength(256, MinimumLength = 1)]
    public string? ErrorMessage { get; set; }
}
