using System.ComponentModel.DataAnnotations;

namespace Samples.Identity.Models.Account;

public record LoginInputModel
{
    public LoginInputModel()
    {
    }

    public LoginInputModel(LoginInputModel model)
    {
        Email = model.Email;
        Password = model.Password;
        RememberMe = model.RememberMe;
        ReturnUrl = model.ReturnUrl;
    }

    [Required, EmailAddress(ErrorMessage = "A valid email address is required")]
    public string? Email { get; set; }

    [Required, DataType(DataType.Password), StringLength(64, MinimumLength = 5)]
    public string? Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; } = true;

    public string? ReturnUrl { get; set; }
}

public record LoginViewModel : LoginInputModel
{
    public LoginViewModel() : base()
    {
    }

    public LoginViewModel(LoginInputModel model) : base(model)
    {
    }
}
