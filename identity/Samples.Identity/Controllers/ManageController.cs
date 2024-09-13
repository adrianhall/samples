using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Samples.Identity.Data;
using Samples.Identity.Extensions;
using Samples.Identity.Models.Manage;

#pragma warning disable IDE0305 // Simplify collection initialization

namespace Samples.Identity.Controllers;

[AutoValidateAntiforgeryToken]
public class ManageController(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<ManageController> logger
    ) : Controller
{
    [HttpGet]
    public IActionResult Roles() => View();

    [HttpGet]
    public IActionResult Users() => View();

    /// <summary>
    /// A processor to return the roles for a datatables.net server side rendering.
    /// </summary>
    /// <param name="model">The input model for the datatables.net call</param>
    /// <returns></returns>
    [HttpGet]
    [Produces("application/json")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> RolesTableHandler(DataTablesInputModel model)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("RolesTableHandler model is invalid: {errorMessages}", string.Join("|", ModelState.GetErrorMessages()));
            return BadRequest(ModelState);
        }

        int totalCount = roleManager.Roles.Count();
        var rolesInView = roleManager.Roles.OrderBy(x => x.NormalizedName);

        // TODO: Search / OrderBy

        int filteredCount = rolesInView.Count();
        IList<RoleViewModel> items = rolesInView.Skip(model.StartIndex).Take(model.Length).Select((role, idx) => new RoleViewModel(role)).ToList();
        foreach (var item in items)
        {
            item.MemberCount = (await userManager.GetUsersInRoleAsync(item.Name!)).Count;
        }

        var results = new BootstrapTableViewModel<RoleViewModel>()
        {
            RequestId = model.RequestId,
            TotalCount = totalCount,
            FilteredCount = filteredCount,
            Items = items
        };

        return Json(results);
    }
}
