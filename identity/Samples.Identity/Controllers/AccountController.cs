using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Samples.Identity.Data;
using Samples.Identity.Models.Account;
using System.Text;
using System.Text.Json;

namespace Samples.Identity.Controllers;

[AutoValidateAntiforgeryToken]
public class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IEmailSender<ApplicationUser> emailSender,
    ILogger<AccountController> logger,
    IWebHostEnvironment environment
    ) : Controller
{
    /// <summary>
    /// Determines if the user should be locked out according to the rules of lockouts.
    /// </summary>
    internal bool LockoutOnFailure { get => !environment.IsDevelopment(); }

    #region Login
    [HttpGet]
    public IActionResult Login([FromQuery] string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");
        return View(new LoginViewModel() { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginInputModel model)
    {
        logger.LogTrace("Login: form = {json}", JsonSerializer.Serialize(model));
        model.ReturnUrl ??= Url.Content("~/");
        if (!ModelState.IsValid || model.Email is null || model.Password is null)
        {
            logger.LogDebug("Login: form is invalid");
            ModelState.AddModelError(string.Empty, "Form is invalid");
            return View(new LoginViewModel(model));
        }

        ApplicationUser? user = await userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            logger.LogDebug("Login: email address {email} not found.", model.Email);
            ModelState.AddModelError(string.Empty, "Unknown user/password combination.");
            return View(new LoginViewModel(model));
        }

        var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: LockoutOnFailure);
        if (result.Succeeded)
        {
            logger.LogDebug("Login: password sign in for {email} succeeded.", model.Email);
            return LocalRedirect(model.ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            logger.LogDebug("Login: email address {email} is locked out", model.Email);
            ModelState.AddModelError(string.Empty, "Locked out.");
            return View(new LoginViewModel(model));
        }

        logger.LogDebug("Login: password sign in for {email} failed.", model.Email);
        ModelState.AddModelError(string.Empty, "Unknown user/password combination.");
        return View(new LoginViewModel(model));
    }
    #endregion

    #region Logout
    [HttpGet]
    public async Task<IActionResult> Logout([FromQuery] string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");
        if (!signInManager.IsSignedIn(HttpContext.User))
        {
            logger.LogTrace("Logout: User is not signed in - redirecting");
            return LocalRedirect(returnUrl);
        }

        ApplicationUser? user = await userManager.GetUserAsync(HttpContext.User);
        if (user is not null)
        {
            logger.LogTrace("Logout: Logging out {email}", user.Email);
        }
        else
        {
            logger.LogTrace("Logout: Cannot determine user record - something is very wrong");
        }

        await signInManager.SignOutAsync();
        return LocalRedirect(returnUrl);
    }
    #endregion

    #region Register
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string? userId, [FromQuery] string? token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return RedirectToHomePage();
        }

        ApplicationUser? user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound($"No such user {userId}.");
        }

        IdentityResult result = await userManager.ConfirmEmailAsync(user, DecodeToken(token));
        if (result.Succeeded)
        {
            return View(new EmailConfirmationViewModel() { Email = user.Email! });
        }

        return RedirectToAction(nameof(ResendEmailConfirmation), new { user.Email });
    }

    /// <summary>
    /// Displays the registration form
    /// </summary>
    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    /// <summary>
    /// Displays the form to resend the email confirmation.
    /// </summary>
    [HttpGet]
    public IActionResult ResendEmailConfirmation([FromQuery] string? email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction(nameof(Register));
        }

        return View(new EmailConfirmationViewModel() { Email = email, Timestamp = DateTimeOffset.UtcNow.Ticks });
    }

    /// <summary>
    /// Handles the form to resend the email confirmation.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ResendEmailConfirmation([FromForm] EmailConfirmationInputModel input)
    {
        logger.LogTrace("Resend email confirmation form submission: {input}", JsonSerializer.Serialize(input));
        if (!ModelState.IsValid || input.Email is null)
        {
            logger.LogDebug("Resend email confirmation form submission: Form was invalid");
            return View(new EmailConfirmationViewModel(input));
        }

        ApplicationUser? existingUser = await userManager.FindByEmailAsync(input.Email);
        if (existingUser is null)
        {
            logger.LogDebug("Resend email confirmation Form Submission: User {email} does not exist", input.Email);
            ModelState.AddModelError(string.Empty, "Email not registered");
            return View(new EmailConfirmationViewModel(input));
        }

        // Rate limiting
        DateTimeOffset lastTimestamp = new(input.Timestamp, TimeSpan.Zero);
        DateTimeOffset nextTimestamp = lastTimestamp.AddMinutes(10);
        if (DateTimeOffset.UtcNow < nextTimestamp)
        {
            logger.LogDebug("Resend email confirmation Form Submission: Rate limiting {email} until {nextTimestamp}", input.Email, nextTimestamp);
            ModelState.AddModelError(string.Empty, "Please wait longer before asking for a new code");
            return View(new EmailConfirmationViewModel(input));
        }

        await SendConfirmationLinkAsync(existingUser);
        return View(new EmailConfirmationViewModel(input) { Timestamp = DateTimeOffset.UtcNow.Ticks });
    }

    /// <summary>
    /// Handles the result from the POST of the registration form.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Register([FromForm] RegisterInputModel input)
    {
        logger.LogTrace("Register Form Submission: {input}", JsonSerializer.Serialize(input));
        if (!ModelState.IsValid || input.Email is null || input.Password is null || input.DisplayName is null)
        {
            logger.LogDebug("Register Form Submission: Form was invalid");
            return View(new RegisterViewModel(input));
        }

        // TODO: If you need to validate DisplayName - e.g. to check it isn't a profane username
        if (IsDisallowedDisplayName(input.DisplayName))
        {
            logger.LogDebug("Register Form Submission: Display name was disallowed");
            ModelState.AddModelError(string.Empty, "The Display Name was disallowed by policy");
            return View(new RegisterViewModel(input));
        }

        ApplicationUser? existingUser = await userManager.FindByEmailAsync(input.Email);
        if (existingUser is not null)
        {
            logger.LogDebug("Register Form Submission: User {email} already exists as {id}", input.Email, existingUser.Id);
            ModelState.AddModelError(string.Empty, "User already registered.");
            return View(new RegisterViewModel());
        }

        ApplicationUser newUser = new()
        {
            Id = Guid.NewGuid().ToString("N"),
            UserName = input.Email.ToLowerInvariant(),
            Email = input.Email.ToLowerInvariant(),
            DisplayName = input.DisplayName
        };

        logger.LogTrace("Register: Creating new user {newUser}", JsonSerializer.Serialize(newUser));
        IdentityResult result = await userManager.CreateAsync(newUser, input.Password);
        if (!result.Succeeded)
        {
            logger.LogError("Register: Could not create {id}: {errors}", newUser.Id, JsonSerializer.Serialize(result.Errors.Select(e => e.Description)));
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(new RegisterViewModel(input));
        }

        // If an email confirmation is NOT required, then sign the user in and redirect to the home page
        if (!userManager.Options.SignIn.RequireConfirmedAccount)
        {
            logger.LogDebug("Register: RequireConfirmedAccount = false; sign-in {email} automatically", newUser.Email);
            await signInManager.SignInAsync(newUser, isPersistent: false);
            return RedirectToHomePage();
        }

        // email confirmation IS required; send the email and display the email confirmation required page
        await SendConfirmationLinkAsync(newUser);
        return RedirectToAction(nameof(ResendEmailConfirmation), new { newUser.Email });
    }
    #endregion

    #region Helper methods
    /// <summary>
    /// Decodes a confirmation or reset password token from a URI so it can be used with userManager.
    /// </summary>
    internal static string DecodeToken(string token)
        => Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

    /// <summary>
    /// Encodes a confirmation or reset password token so that it can be used in a URI.
    /// </summary>
    internal static string EncodeToken(string token)
        => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

    /// <summary>
    /// Determines if the display name is disallowed.
    /// </summary>
    internal static bool IsDisallowedDisplayName(string displayName)
    {
        var filter = new ProfanityFilter.ProfanityFilter();
        return filter.IsProfanity(displayName);
    }

    /// <summary>
    /// Returns an <see cref="IActionResult"/> to redirect to the home page.
    /// </summary>
    internal IActionResult RedirectToHomePage()
        => LocalRedirect(Url.Content("~/"));

    /// <summary>
    /// Sends the registration confirmation link to the specified user.
    /// </summary>
    internal async Task SendConfirmationLinkAsync(ApplicationUser user)
    {
        logger.LogTrace("SendConfirmationLink: {json}", JsonSerializer.Serialize(user));
        string userId = await userManager.GetUserIdAsync(user);
        string confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        string? callbackUrl = Url.ActionLink(
            action: nameof(ConfirmEmail),
            values: new { userId, token = EncodeToken(confirmationToken) },
            protocol: Request.Scheme);
        await emailSender.SendConfirmationLinkAsync(user, user.Email!, callbackUrl!);
    }
    #endregion
}
