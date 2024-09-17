using System.Text.Json.Serialization;

namespace Samples.Identity.Models;

public record EmailAddress
{
    public EmailAddress()
    {
        Email = string.Empty;
    }

    public EmailAddress(string address)
    {
        Email = address;
    }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("name")]
    public string? DisplayName { get; set; }
}
