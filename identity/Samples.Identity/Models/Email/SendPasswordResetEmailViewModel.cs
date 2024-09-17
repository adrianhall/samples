namespace Samples.Identity.Models.Email;

public record SendPasswordResetEmailViewModel : EmailButtonViewModel
{
    public SendPasswordResetEmailViewModel() : base("Reset password")
    {
    }

    public string EmailTitle { get; set; } = "Reset your password";

    public string EmailContent { get; set; } = "Click the button below to reset your password.";
}
