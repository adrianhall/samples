using Microsoft.AspNetCore.Identity;

namespace Samples.Identity.Models.Manage;

public record RoleViewModel
{
    public RoleViewModel()
    {
    }

    public RoleViewModel(IdentityRole role)
    {
        Id = role.Id;
        Name = role.Name;
        NormalizedName = role.NormalizedName;
    }

    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? NormalizedName { get; set; }
    public long MemberCount { get; set; }
}
