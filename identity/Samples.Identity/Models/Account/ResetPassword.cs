using System.ComponentModel.DataAnnotations;

namespace Samples.Identity.Models.Account;

public record ResetPasswordInputModel
{
    public ResetPasswordInputModel()
    {
    }

    public ResetPasswordInputModel(ResetPasswordInputModel inputModel)
    {
        Email = inputModel.Email;
        Password = inputModel.Password;
        ConfirmPassword = inputModel.ConfirmPassword;
        Token = inputModel.Token;
        ReturnUrl = inputModel.ReturnUrl;
    }

    [Required, EmailAddress, StringLength(256, MinimumLength = 3)]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), StringLength(64, MinimumLength = 3)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), StringLength(64, MinimumLength = 3)]
    [Compare(nameof(Password), ErrorMessage = "Password and confirmation must match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;
}

public record ResetPasswordViewModel : ResetPasswordInputModel
{
    public ResetPasswordViewModel()
    {
    }

    public ResetPasswordViewModel(ResetPasswordInputModel inputModel) : base(inputModel)
    {
    }
}
