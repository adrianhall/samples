using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Samples.Identity.Data;
using Samples.Identity.Models.Account;

namespace Samples.Identity.Controllers;

[AutoValidateAntiforgeryToken]
public class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ILogger<AccountController> logger
    ) : Controller
{
    #region Register
    /// <summary>
    /// Displays the registration form
    /// </summary>
    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());
    #endregion
}
