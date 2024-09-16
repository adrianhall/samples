using Microsoft.AspNetCore.Identity;

namespace Samples.Identity.Data;

/// <summary>
/// The form of the application user additional fields.
/// </summary>
public interface IApplicationUser
{
    string FirstName { get; set; }
    string LastName { get; set; }
    string DisplayName { get; set; }
}

public class ApplicationUser : IdentityUser, IApplicationUser
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;
}
