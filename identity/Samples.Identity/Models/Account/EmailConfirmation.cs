using System.ComponentModel.DataAnnotations;

namespace Samples.Identity.Models.Account;

public record EmailConfirmationInputModel
{
    public EmailConfirmationInputModel()
    {
    }

    public EmailConfirmationInputModel(EmailConfirmationInputModel input)
    {
        Email = input.Email;
        Timestamp = input.Timestamp;
    }

    [Required, EmailAddress]
    public string? Email { get; set; }

    public long Timestamp { get; set; } = 0L;
}

public record EmailConfirmationViewModel : EmailConfirmationInputModel
{
    public EmailConfirmationViewModel() : base()
    {
    }

    public EmailConfirmationViewModel(EmailConfirmationInputModel input) : base(input)
    {
    }
}
