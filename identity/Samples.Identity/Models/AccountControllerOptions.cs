namespace Samples.Identity.Models;

/// <summary>
/// The options for the account controller
/// </summary>
public class AccountControllerOptions
{
    /// <summary>
    /// <c>true</c> if lockout support is enabled.
    /// </summary>
    public bool EnableLockoutSupport { get; set; } = false;
}
