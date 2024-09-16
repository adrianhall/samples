using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Samples.Identity.Data;
using Samples.Identity.Extensions;
using Samples.Identity.Models;
using Samples.Identity.Models.Account;
using System.Text;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Samples.Identity.Controllers;

[AutoValidateAntiforgeryToken]
public class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IEmailSender<ApplicationUser> emailSender,
    IOptions<AccountControllerOptions> options,
    ILogger<AccountController> logger
    ) : Controller
{
    /// <summary>
    /// The page after email confirmation is sent.
    /// </summary>
    /// <param name="email">The email address that the confirmation was sent to.</param>
    /// <param name="returnUrl">The return URL requested.</param>
    [HttpGet, AllowAnonymous]
    public IActionResult AwaitEmailConfirmation(string email, string returnUrl)
    {
        logger.LogTrace("GET AwaitEmailConfirmation: {email} {returnUrl}", email, returnUrl);
        return View(new EmailConfirmationViewModel { Email = email, ReturnUrl = returnUrl });
    }

    /// <summary>
    /// The page after a reset password link is sent.
    /// </summary>
    /// <param name="email">The email address that the confirmation was sent to.</param>
    /// <param name="returnUrl">The return URL requested.</param>
    [HttpGet, AllowAnonymous]
    public IActionResult AwaitPasswordReset(string email, string returnUrl)
    {
        logger.LogTrace("GET AwaitPasswordResewt: {email} {returnUrl}", email, returnUrl);
        return View(new EmailConfirmationViewModel { Email = email, ReturnUrl = returnUrl });
    }

    /// <summary>
    /// Processes the emailed confirmation link.
    /// </summary>
    /// <param name="userId">The user ID from the link.</param>
    /// <param name="code">The confirmation code from the link.</param>
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string? userId, [FromQuery] string? code)
    {
        logger.LogTrace("GET ConfirmEmail: {userId} {code}", userId, code);
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
        {
            logger.LogWarning("ConfirmEmail: UserId or code is null or empty.");
            return RedirectToHomePage();
        }

        ApplicationUser? user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            logger.LogWarning("ConfirmEmail: User {userId} not found.", userId);
            return RedirectToHomePage();
        }

        try
        {
            string token = DecodeToken(code);
            IdentityResult result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                logger.LogWarning("ConfirmEmail: Could not confirm email for user {userId}: {errors}", userId, result.Errors);
                return RedirectToAction(
                    nameof(ResendEmailConfirmation),
                    new { user.Email }
                );
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning("ConfirmEmail: Could not confirm email for user {userId}: {exception}", userId, ex);
            return RedirectToAction(
                nameof(ResendEmailConfirmation),
                new { user.Email }
            );
        }

        // User has confirmed their email, so we can now sign them in.
        await signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToHomePage();
    }

    /// <summary>
    /// Displays the "forgot password" dialog.
    /// </summary>
    /// <param name="returnUrl">Where to redirect after completion.</param>
    [HttpGet, AllowAnonymous]
    public IActionResult ForgotPassword([FromQuery] string? returnUrl)
    {
        logger.LogTrace("GET ForgotPassword: {returnUrl}", returnUrl);

        returnUrl = HomePageIfNullOrEmpty(returnUrl);
        if (signInManager.IsSignedIn(User))
        {
            logger.LogDebug("Login: User is already signed in.");
            return RedirectToHomePage();
        }

        EmailConfirmationViewModel viewModel = new()
        {
            ReturnUrl = returnUrl
        };

        return View(viewModel);
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword([FromForm] EmailConfirmationInputModel model)
    {
        logger.LogTrace("POST ForgotPassword: {json}", model.ToJsonString());

        model.ReturnUrl = HomePageIfNullOrEmpty(model.ReturnUrl);
        if (!ModelState.IsValid)
        {
            EmailConfirmationViewModel viewModel = new(model);
            LogAllModelStateErrors(ModelState);
            return View(viewModel);
        }

        ApplicationUser? user = await userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            logger.LogDebug("ForgotPassword: User '{email}' not found.", model.Email);
            return RedirectToAction(nameof(AwaitPasswordReset), new { model.Email, model.ReturnUrl });
        }

        await SendResetPasswordLinkAsync(user);
        return RedirectToAction(
            nameof(AwaitPasswordReset),
            new { model.Email, model.ReturnUrl }
        );

    }

    /// <summary>
    /// Displays the locked out page to the user.
    /// </summary>
    [HttpGet, AllowAnonymous]
    public IActionResult LockedOut()
        => View();

    /// <summary>
    /// Displays the blank login page to the user.
    /// </summary>
    /// <param name="returnUrl">Where the user should be redirected back to.</param>
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        logger.LogTrace("GET Login: {returnUrl}", returnUrl);

        returnUrl = HomePageIfNullOrEmpty(returnUrl);
        if (signInManager.IsSignedIn(User))
        {
            logger.LogDebug("Login: User is already signed in.");
            return RedirectToHomePage();
        }

        LoginViewModel viewModel = new()
        {
            ReturnUrl = returnUrl,
            ExternalProviders = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
        };

        return View(viewModel);
    }

    /// <summary>
    /// Processed the login form submission.
    /// </summary>
    /// <param name="model">The form submission</param>
    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromForm] LoginInputModel model)
    {
        logger.LogTrace("POST Login: {json}", model.ToJsonString());

        async Task<IActionResult> DisplayLoginView()
        {
            LoginViewModel viewModel = new(model)
            {
                ExternalProviders = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };
            return View(viewModel);
        }

        model.ReturnUrl = HomePageIfNullOrEmpty(model.ReturnUrl);
        if (!ModelState.IsValid)
        {
            LogAllModelStateErrors(ModelState);
            return await DisplayLoginView();
        }

        ApplicationUser? user = await userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            logger.LogDebug("Login: User '{email}' not found.", model.Email);
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return await DisplayLoginView();
        }

        SignInResult result = await signInManager.PasswordSignInAsync(
            user, model.Password,
            isPersistent: model.RememberMe,
            lockoutOnFailure: options.Value.EnableLockoutSupport
        );
        if (result.Succeeded)
        {
            logger.LogDebug("Login: User logged in.");
            return RedirectToHomePage();
        }

        // if (result.RequiresTwoFactor)
        // {
        //     logger.LogDebug("Login: User '{email}' requires two-factor authentication.", model.Email);
        //     return RedirectToAction(
        //         nameof(TwoFactorAuthentication),
        //         new { model.ReturnUrl, model.RememberMe }
        //     );
        // }

        if (result.IsLockedOut)
        {
            logger.LogDebug("Login: User '{email}' is locked out.", model.Email);
            return RedirectToAction(nameof(LockedOut));
        }

        logger.LogDebug("Login: Invalid username/password for user '{email}'.", model.Email);
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return await DisplayLoginView();
    }

    /// <summary>
    /// Logs the signed in user out.
    /// </summary>
    /// <param name="returnUrl">Where to redirect afterwards</param>
    [HttpGet, Authorize]
    public async Task<IActionResult> Logout([FromQuery] string? returnUrl = null)
    {
        logger.LogTrace("GET Logout: {returnUrl}", returnUrl);

        returnUrl = HomePageIfNullOrEmpty(returnUrl);
        if (!signInManager.IsSignedIn(User))
        {
            logger.LogTrace("Logout: User is not signed in.");
            return Redirect(returnUrl);
        }

        ApplicationUser? user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            logger.LogWarning("Logout: User is signed in but not found.");
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        logger.LogDebug("Logout: User '{email}' signed out.", user.Email);
        await signInManager.SignOutAsync();
        return Redirect(returnUrl);
    }

    /// <summary>
    /// Displays the registration page.
    /// </summary>
    /// <param name="returnUrl">Where to redirect after the registration process is complete.</param>
    [HttpGet, AllowAnonymous]
    public IActionResult Register([FromQuery] string? returnUrl = null)
    {
        logger.LogTrace("GET Register: {returnUrl}", returnUrl);

        returnUrl = HomePageIfNullOrEmpty(returnUrl);
        if (signInManager.IsSignedIn(User))
        {
            logger.LogDebug("Register: User is already signed in.");
            return Redirect(returnUrl);
        }

        RegisterViewModel viewModel = new()
        {
            ReturnUrl = returnUrl
        };

        return View(viewModel);
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromForm] RegisterInputModel model)
    {
        logger.LogTrace("POST Register: {json}", model.ToJsonString());

        async Task<IActionResult> DisplayRegisterView()
        {
            RegisterViewModel viewModel = new(model)
            {
                ExternalProviders = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };
            return View(viewModel);
        }

        model.ReturnUrl = HomePageIfNullOrEmpty(model.ReturnUrl);
        if (!ModelState.IsValid)
        {
            LogAllModelStateErrors(ModelState);
            return await DisplayRegisterView();
        }

        // TODO: Check to see if the IApplicationUser fields are valid

        ApplicationUser user = new()
        {
            Id = Guid.NewGuid().ToString("N"),
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            DisplayName = model.DisplayName
        };

        logger.LogTrace("Register: Creating user {json}", user.ToJsonString());
        IdentityResult result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            logger.LogError("Register: Could not create user {json}: {errors}", user.ToJsonString(), result.Errors);
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return await DisplayRegisterView();
        }

        if (!userManager.Options.SignIn.RequireConfirmedAccount)
        {
            logger.LogDebug("Register: RequireConfirmedAccount = false; sign-in {email} automatically", user.Email);
            await signInManager.SignInAsync(user, isPersistent: false);
            return Redirect(model.ReturnUrl);
        }

        await SendConfirmationLinkAsync(user);
        return RedirectToAction(
            nameof(AwaitEmailConfirmation),
            new { model.Email, model.ReturnUrl }
        );
    }

    [HttpGet, AllowAnonymous]
    public IActionResult ResendEmailConfirmation([FromQuery] string? email)
    {
        logger.LogTrace("GET ResendEmailConfirmation: {email}", email);
        if (string.IsNullOrWhiteSpace(email))
        {
            logger.LogWarning("ResendEmailConfirmation: Email is null or empty.");
            return RedirectToHomePage();
        }

        return View(new EmailConfirmationViewModel { Email = email });
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendEmailConfirmation([FromForm] EmailConfirmationInputModel model)
    {
        logger.LogTrace("POST ResendEmailConfirmation: {json}", model.ToJsonString());

        model.ReturnUrl = HomePageIfNullOrEmpty(model.ReturnUrl);
        if (!ModelState.IsValid)
        {
            LogAllModelStateErrors(ModelState);
            return View(model);
        }

        ApplicationUser? user = await userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            logger.LogWarning("ResendEmailConfirmation: User '{email}' not found.", model.Email);
            ModelState.AddModelError(string.Empty, "Invalid email address.");
            return View(model);
        }

        await SendConfirmationLinkAsync(user);
        return RedirectToAction(
            nameof(AwaitEmailConfirmation),
            new { model.Email, model.ReturnUrl }
        );
    }

    /// <summary>
    /// Processes the emailed password reset.
    /// </summary>
    /// <param name="userId">The user ID from the link.</param>
    /// <param name="code">The confirmation code from the link.</param>
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromQuery] string? userId, [FromQuery] string? code)
    {
        logger.LogTrace("GET ResetPassword: {userId} {code}", userId, code);
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
        {
            logger.LogWarning("ResetPassword: UserId or code is null or empty.");
            return RedirectToHomePage();
        }

        ApplicationUser? user = await userManager.FindByIdAsync(userId);
        if (user is null || user.Email is null)
        {
            logger.LogWarning("ResetPassword: User {userId} not found.", userId);
            return RedirectToHomePage();
        }

        string resetToken = DecodeToken(code);
        ResetPasswordViewModel viewModel = new() { Email = user.Email, Token = resetToken };
        return View(viewModel);
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordInputModel model)
    {
        logger.LogTrace("POST ResetPassword: {json}", model.ToJsonString());

        IActionResult DisplayView()
        {
            ResetPasswordViewModel viewModel = new(model);
            return View(viewModel);
        }

        model.ReturnUrl = HomePageIfNullOrEmpty(model.ReturnUrl);
        if (!ModelState.IsValid)
        {
            LogAllModelStateErrors(ModelState);
            return DisplayView();
        }

        ApplicationUser? user = await userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            logger.LogWarning("ResetPassword: User '{email}' not found.", model.Email);
            ModelState.AddModelError(string.Empty, "Invalid email address.");
            return DisplayView();
        }

        IdentityResult result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (!result.Succeeded)
        {
            logger.LogWarning("ResetPassword: Could not reset password for {email}:", model.Email);
            foreach (IdentityError error in result.Errors)
            {
                logger.LogWarning("Error: {errorDescription}", error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return DisplayView();
        }

        await signInManager.SignInAsync(user, isPersistent: false);
        return Redirect(model.ReturnUrl);
    }


    #region Helpers
    /// <summary>
    /// Converts the return URL to the home page if it is null or empty.
    /// </summary>
    /// <param name="returnUrl">The return URL</param>
    /// <returns>The modeified return URL</returns>
    internal string HomePageIfNullOrEmpty(string? returnUrl)
        => string.IsNullOrWhiteSpace(returnUrl) ? Url.Content("~/") : returnUrl;

    /// <summary>
    /// Logs all the errors within a model state dictionary.
    /// </summary>
    /// <param name="modelState">The model state dictionary to log.</param>
    internal void LogAllModelStateErrors(ModelStateDictionary modelState)
    {
        foreach (ModelError error in modelState.Values.SelectMany(v => v.Errors))
        {
            logger.LogDebug("ModelState: {error}", error.ErrorMessage);
        }
    }

    /// <summary>
    /// Sends the registration confirmation link to the user.
    /// </summary>
    /// <param name="user">The user record.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A task that completes when the operation is completed.</returns>
    internal Task SendConfirmationLinkAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("SendConfirmationLink: {json}", user.ToJsonString());
        return Task.Run(async () =>
        {
            string userId = await userManager.GetUserIdAsync(user);
            string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            logger.LogTrace("SendConfirmationLink: {userId} {token}", userId, token);
            string? callbackUrl = Url.ActionLink(
                action: nameof(ConfirmEmail),
                values: new { userId, code = EncodeToken(token) },
                protocol: Request.Scheme
            );
            logger.LogTrace("SendConfirmationLink: {userId} {callbackUrl}", userId, callbackUrl);
            if (callbackUrl is null || user.Email is null)
            {
                logger.LogError("Failed to generate registration confirmation link for user {userId}", userId);
                throw new ApplicationException("Failed to generate registration confirmation link.");
            }
            await emailSender.SendConfirmationLinkAsync(user, user.Email, callbackUrl);
        }, cancellationToken);
    }

    /// <summary>
    /// Sends the registration confirmation link to the user.
    /// </summary>
    /// <param name="user">The user record.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A task that completes when the operation is completed.</returns>
    internal Task SendResetPasswordLinkAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("SendResetPasswordLink: {json}", user.ToJsonString());
        return Task.Run(async () =>
        {
            string userId = await userManager.GetUserIdAsync(user);
            string token = await userManager.GeneratePasswordResetTokenAsync(user);
            logger.LogTrace("SendResetPasswordLink: {userId} {token}", userId, token);
            string? callbackUrl = Url.ActionLink(
                action: nameof(ResetPassword),
                values: new { userId, code = EncodeToken(token) },
                protocol: Request.Scheme
            );
            logger.LogTrace("SendResetPasswordLink: {userId} {callbackUrl}", userId, callbackUrl);
            if (callbackUrl is null || user.Email is null)
            {
                logger.LogError("Failed to generate password reset link for user {userId}", userId);
                throw new ApplicationException("Failed to generate password reset link.");
            }
            await emailSender.SendPasswordResetLinkAsync(user, user.Email, callbackUrl);
        }, cancellationToken);
    }

    /// <summary>
    /// Decodes an incoming token back to a form that can be validated by ASP.NET Identity.
    /// </summary>
    /// <param name="code">The code from the user.</param>
    /// <returns>The ASP.NET Identity token.</returns>
    internal static string DecodeToken(string code)
        => Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

    /// <summary>
    /// Encodes a confirmation token to a form that can be sent to a user as a URL.
    /// </summary>
    /// <param name="token">The token to encode for the user.</param>
    /// <returns>The code to be sent to the user.</returns>
    internal static string EncodeToken(string token)
        => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

    /// <summary>
    /// Returns an <see cref="IActionResult"/> that redirects the user to the home page.
    /// </summary>
    internal IActionResult RedirectToHomePage()
        => LocalRedirect(Url.Content("~/"));
    #endregion
}
