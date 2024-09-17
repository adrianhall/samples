using System.Text.Json.Serialization;

namespace Samples.Identity.Models;

public record EmailMessage
{
    [JsonPropertyName("from")]
    public EmailAddress? FromAddress { get; set; }

    [JsonPropertyName("to")]
    public IList<EmailAddress> ToAddresses { get; set; } = [];

    [JsonPropertyName("subject")]
    public required string Subject { get; set; }

    [JsonPropertyName("text")]
    public required string TextContent { get; set; }

    [JsonPropertyName("html")]
    public string? HtmlContent { get; set; }
}
