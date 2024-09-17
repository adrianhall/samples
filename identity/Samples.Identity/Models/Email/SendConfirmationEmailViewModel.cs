namespace Samples.Identity.Models.Email;

public record SendConfirmationEmailViewModel : EmailButtonViewModel
{
    public SendConfirmationEmailViewModel() : base("Confirm registration")
    {
    }

    public string EmailTitle { get; set; } = "Confirm your account";

    public string EmailContent { get; set; } = "Click the button below to confirm your account.";
}
