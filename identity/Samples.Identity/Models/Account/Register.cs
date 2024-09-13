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

    [Required, EmailAddress(ErrorMessage = "A valid email address is required.")]
    public string? Email { get; set; }

    [Required, StringLength(100, MinimumLength = 1, ErrorMessage = "Enter a name of between 1 and 100 characters")]
    [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "Only alphanumeric characters and the space are allowed")]
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
