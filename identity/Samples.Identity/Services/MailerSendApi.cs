using Microsoft.Extensions.Options;
using Samples.Identity.Extensions;
using Samples.Identity.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Samples.Identity.Services;

public class MailerSendApi(
    IOptions<MailerSendOptions> options,
    ILogger<MailerSendApi> logger
    ) : ISendEmailApi
{
    internal const string MailerSendUri = "https://api.mailersend.com/v1/email";
    internal static MediaTypeHeaderValue jsonMediaType = MediaTypeHeaderValue.Parse( "application/json" );
    internal static JsonSerializerOptions serializerOptions = GetSerializerOptions();
    internal HttpClient client = new();

    /// <summary>
    /// The API Key for the MailerSend platform, or null.
    /// </summary>
    internal string? ApiKey { get => string.IsNullOrWhiteSpace(options.Value?.ApiKey) ? null : options.Value.ApiKey; }

    /// <summary>
    /// The "from" email address for sending emails through the MailerSend platform, or null.
    /// </summary>
    internal string? FromEmail { get => string.IsNullOrWhiteSpace(options.Value?.FromEmail) ? null : options.Value.FromEmail; }

    /// <summary>
    /// The "from" display name for sending emails through the MailerSend platform, or null.
    /// </summary>
    internal string? FromName { get => string.IsNullOrWhiteSpace(options.Value?.FromName) ? null : options.Value.FromName; }

    /// <summary>
    /// Determines if there is a valid configuration specified.
    /// </summary>
    public bool IsConfigured { get => ApiKey is not null && FromEmail is not null; }

    /// <summary>
    /// Sends an email through a validly configured email sender API.
    /// </summary>
    /// <param name="message">The message to send</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    /// <returns>The results of the email send operation.</returns>
    public async Task<EmailResult> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("SendEmailAsync: {message}", message.ToJsonString());
        if (!IsConfigured)
        {
            logger.LogError("MailerSendApi has not been configured.");
            throw new InvalidOperationException("MailerSend is not configured correctly.");
        }

        // Force the From Address, irrespective of what the email message says.
        message.FromAddress = new EmailAddress(FromEmail!) { DisplayName = FromName };

        using HttpRequestMessage request = new(HttpMethod.Post, new Uri(MailerSendUri));
        request.Content = JsonContent.Create(message, jsonMediaType, serializerOptions);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

        using HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
        EmailResult result = new()
        { 
            Succeeded = response.IsSuccessStatusCode,
            ResultCode = (int)response.StatusCode,
            Messages = [
                await response.Content.ReadAsStringAsync(cancellationToken)
            ]
        };

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Email successfully submitted: {result}", result.ToJsonString());
        }
        else
        {
            string messageId = response.Headers.GetValues("x-message-id").FirstOrDefault() ?? "not-returned";
            logger.LogError("Email submission failed. {statusCode} {reasonPhrase} x-message-id={messageId}", 
                response.StatusCode, response.ReasonPhrase, messageId);
        }

        return result;
    }

    internal static JsonSerializerOptions GetSerializerOptions()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
        {
            AllowTrailingCommas = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.Strict,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return options;
    }
}
