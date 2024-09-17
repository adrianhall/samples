namespace Samples.Identity.Models.Email;

public record EmailButtonViewModel(string text)
{
    public required string Url { get; set; }
    public string Text { get; set; } = text;
}
