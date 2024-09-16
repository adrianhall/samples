using System.ComponentModel.DataAnnotations;

namespace Samples.Identity.Models.Account;

public record EmailConfirmationInputModel
{
    public EmailConfirmationInputModel()
    {
    }

    public EmailConfirmationInputModel(EmailConfirmationInputModel inputModel)
    {
        Email = inputModel.Email;
        ReturnUrl = inputModel.ReturnUrl;
    }

    [Required, EmailAddress, StringLength(256, MinimumLength = 3)]
    public string Email { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;
}

public record EmailConfirmationViewModel : EmailConfirmationInputModel
{
    public EmailConfirmationViewModel()
    {
    }

    public EmailConfirmationViewModel(EmailConfirmationInputModel inputModel) : base(inputModel)
    {
    }
}
