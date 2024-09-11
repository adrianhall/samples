using System.ComponentModel.DataAnnotations;

namespace Samples.Identity.Models.Account;

public record RegisterInputModel
{
    public RegisterInputModel()
    {
    }

    public RegisterInputModel(RegisterInputModel model)
    {
        Email = model.Email;
        Password = model.Password;
        ConfirmPassword = model.ConfirmPassword;
        DisplayName = model.DisplayName;
    }

    [Required, EmailAddress]
    public string? Email { get; set; }

    [Required, MinLength(1), MaxLength(100)]
    public string? DisplayName { get; set; }

    [Required, DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required, DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Password and confirmation must match")]
    public string? ConfirmPassword { get; set; }
}

public record RegisterViewModel : RegisterInputModel
{
    public RegisterViewModel() : base()
    {
    }

    public RegisterViewModel(RegisterInputModel inputModel) : base(inputModel)
    {
    }
}
