using Microsoft.AspNetCore.Identity;
using Samples.Identity.Data;

namespace Samples.Identity.Services;

/// <summary>
/// An example <see cref="IEmailSender{TUser}"/> implementation that just logs the request.
/// </summary>
/// <param name="logger">The logger to use.</param>
public class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender<ApplicationUser>
{
    /// <inheritdoc />
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        logger.LogInformation("SendConfirmationLink: email={email},link={confirmationLink}", email, confirmationLink);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        logger.LogInformation("SendPasswordResetCode: email={email},code={resetCode}", email, resetCode);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        logger.LogInformation("SendPasswordResetLink: email={email},link={resetLink}", email, resetLink);
        return Task.CompletedTask;
    }
}
