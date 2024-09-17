namespace Samples.Identity.Models;

public class MailerSendOptions
{
    public string ApiKey { get; set; } = string.Empty;

    public string FromEmail { get; set; } = string.Empty;

    public string FromName { get; set; } = string.Empty;
}
