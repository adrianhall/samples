using Microsoft.AspNetCore.Identity;

namespace Samples.Identity.Data;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
