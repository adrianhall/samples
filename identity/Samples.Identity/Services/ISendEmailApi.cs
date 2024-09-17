using Samples.Identity.Models;

namespace Samples.Identity.Services;

public interface ISendEmailApi
{
    /// <summary>
    /// Determines if the email API is configured correctly.
    /// </summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Sends an email through a validly configured email sender API.
    /// </summary>
    /// <param name="message">The message to send</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    /// <returns>The results of the email send operation.</returns>
    Task<EmailResult> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);


}

public static class ISendEmailApiExtensions
{
    /// <summary>
    /// Sends an email through a validly configured email sender API.
    /// </summary>
    /// <param name="address">The email address to send the email to.</param>
    /// <param name="subject">The subject of the email message.</param>
    /// <param name="textContent">The plain text content</param>
    /// <param name="htmlContent">The HTML content</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    /// <returns>The results of the email send operation.</returns>
    public static Task<EmailResult> SendEmailAsync(this ISendEmailApi api, EmailAddress address, string subject, string textContent, string htmlContent, CancellationToken cancellationToken = default)
    {
        EmailMessage message = new()
        {
            ToAddresses = [address],
            Subject = subject,
            TextContent = textContent,
            HtmlContent = htmlContent
        };

        return api.SendEmailAsync(message, cancellationToken);
    }
}
