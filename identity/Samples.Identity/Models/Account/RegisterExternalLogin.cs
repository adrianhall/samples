using Microsoft.AspNetCore.Authentication;
using Samples.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace Samples.Identity.Models.Account;

public record RegisterExternalLoginInputModel : IApplicationUser
{
    public RegisterExternalLoginInputModel()
    {
    }

    public RegisterExternalLoginInputModel(RegisterExternalLoginInputModel inputModel)
    {
        Email = inputModel.Email;
        FirstName = inputModel.FirstName;
        LastName = inputModel.LastName;
        DisplayName = inputModel.DisplayName;
        ReturnUrl = inputModel.ReturnUrl;
    }

    [Required, EmailAddress, StringLength(256, MinimumLength = 3)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(64, MinimumLength = 1)]
    [RegularExpression(@"^[\w'\-,.]*[^_!¡?÷?¿\/\\+=@#$%ˆ&*(){}|~<>;:[\]]*$")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(64, MinimumLength = 1)]
    [RegularExpression(@"^[\w'\-,.]*[^_!¡?÷?¿\/\\+=@#$%ˆ&*(){}|~<>;:[\]]*$")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression(@"^[\w'\-,.]*[^_!¡?÷?¿\/\\+=@#$%ˆ&*(){}|~<>;:[\]]*$")]
    public string DisplayName { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;
}

public record RegisterExternalLoginViewModel : RegisterExternalLoginInputModel
{
    public RegisterExternalLoginViewModel()
    {
    }

    public RegisterExternalLoginViewModel(RegisterExternalLoginInputModel inputModel) : base(inputModel)
    {
    }

    public string? ProviderDisplayName { get; set; }
}