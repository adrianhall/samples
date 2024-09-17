using Microsoft.AspNetCore.Identity;
using Samples.Identity.Data;
using Samples.Identity.Models;
using Samples.Identity.Models.Email;

namespace Samples.Identity.Services;

public class MailerSendEmailSender(
    ISendEmailApi sendEmailApi,
    IRazorViewToStringRenderer templateRenderer,
    ILogger<MailerSendEmailSender> logger
    ) : IEmailSender<ApplicationUser>
{
    /// <inheritdoc />
    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        logger.LogInformation("SendConfirmationLink: email={email},link={confirmationLink}", email, confirmationLink);

        SendConfirmationEmailViewModel viewModel = new() { Url = confirmationLink };
        string htmlContent = await templateRenderer.RenderViewToStringAsync("/Views/EmailTemplates/SendConfirmationLink.cshtml", viewModel);
        string textContent = $"Your confirmation link is {confirmationLink} - click or copy into your browser.";
        string subject = "Your confirmation link from Samples.Identity";

        EmailAddress address = new(email) { DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? null : user.DisplayName };
        EmailResult result = await sendEmailApi.SendEmailAsync(address, subject, textContent, htmlContent);
        result.LogResult(logger);
    }

    /// <inheritdoc />
    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        logger.LogInformation("SendPasswordResetCode: email={email},code={resetCode}", email, resetCode);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        logger.LogInformation("SendPasswordResetLinkAsync: email={email},link={confirmationLink}", email, resetLink);

        SendPasswordResetEmailViewModel viewModel = new() { Url = resetLink };
        string htmlContent = await templateRenderer.RenderViewToStringAsync("/Views/EmailTemplates/SendPasswordResetLink.cshtml", viewModel);
        string textContent = $"Your password reset link is {resetLink} - click or copy into your browser.";
        string subject = "Your password reset link from Samples.Identity";

        EmailAddress address = new(email) { DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? null : user.DisplayName };
        EmailResult result = await sendEmailApi.SendEmailAsync(address, subject, textContent, htmlContent);
        result.LogResult(logger);
    }
}
