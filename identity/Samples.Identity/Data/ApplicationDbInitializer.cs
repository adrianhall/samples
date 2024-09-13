using Microsoft.AspNetCore.Identity;

namespace Samples.Identity.Data;

public class ApplicationDbInitializer(
    ApplicationDbContext context,
    IWebHostEnvironment env,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager
    ) : IDbInitializer
{
    private const string defaultPassword = "Pass123$";

    /// <summary>
    /// Initialize the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A task that resolves when complete.</returns>
    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken = default)
    {
        // If this is in development, then create the database.
        // If not, you should be doing this externally via migrations or SQL scripts.
        if (!env.IsDevelopment())
        {
            return;
        }

        await context.Database.EnsureCreatedAsync(cancellationToken);
        await SeedUsersAndRolesAsync(cancellationToken);
    }

    /// <summary>
    /// Seeds the users and roles into the provided user manager and role manager.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A task that resolves when complete.</returns>
    internal async Task SeedUsersAndRolesAsync(CancellationToken cancellationToken = default)
    {
        List<string> rolesToSeed = ["admin", "author"];
        List<SeedUser> usersToSeed = [
            new SeedUser("claire", "Claire Redfield", Roles: ["admin"]),
            new SeedUser("ada", "Ada Wong", Roles: ["author"]),
            new SeedUser("luther", "Luther West"),
            new SeedUser("jill", "Jill Valentine")
        ];

        foreach (string roleName in rolesToSeed)
        {
            await EnsureRoleExistsAsync(roleName);
        }

        foreach (SeedUser user in usersToSeed)
        {
            await EnsureUserExistsAsync(user);

            if (user.Roles is not null)
            {
                await EnsureUserIsInRolesAsync(user);
            }
        }
    }

    /// <summary>
    /// Creates a role if it does not exist.
    /// </summary>
    /// <param name="roleName">The name of the role.</param>
    /// <returns>A task that resolves when the operation is complete.</returns>
    internal async Task EnsureRoleExistsAsync(string roleName)
    {
        IdentityRole? role = await roleManager.FindByNameAsync(roleName);
        if (role is not null)
        {
            return;
        }

        IdentityRole newRole = new() { Id = Guid.NewGuid().ToString(), Name = roleName, NormalizedName = roleName.ToLowerInvariant() };
        IdentityResult? result = await roleManager.CreateAsync(newRole);
        ThrowIfNotSuccessful(result, $"Could not create role '{roleName}'");
    }

    /// <summary>
    /// Creates a user if it does not exist.
    /// </summary>
    /// <param name="roleName">The SeedUser record.</param>
    /// <returns>A task that resolves when the operation is complete.</returns>
    internal async Task EnsureUserExistsAsync(SeedUser userRecord)
    {
        ApplicationUser? user = await userManager.FindByNameAsync(userRecord.UserName);
        if (user is not null)
        {
            return;
        }

        ApplicationUser newUser = new()
        {
            Id = Guid.NewGuid().ToString(),
            Email = $"{userRecord.UserName.ToLowerInvariant()}@contoso-email.com",
            EmailConfirmed = true,
            UserName = $"{userRecord.UserName.ToLowerInvariant()}@contoso-email.com",
            DisplayName = userRecord.DisplayName
        };
        IdentityResult? result = await userManager.CreateAsync(newUser, defaultPassword);
        ThrowIfNotSuccessful(result, $"Could not create user '{userRecord.UserName}'");
    }

    /// <summary>
    /// Ensures that the user belongs to the roles (and only the roles) specified.
    /// </summary>
    /// <param name="roleName">The SeedUser record.</param>
    /// <returns>A task that resolves when the operation is complete.</returns>
    internal async Task EnsureUserIsInRolesAsync(SeedUser userRecord)
    {
        string email = $"{userRecord.UserName.ToLowerInvariant()}@contoso-email.com";
        ApplicationUser? user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new ApplicationException($"User '{userRecord.UserName}' does not exist.");
        }

        // There is no ClearRolesAsync(), so let's just remove them if they are present
        IList<string> roles = await userManager.GetRolesAsync(user);
        if (roles.Count > 0)
        {
            IdentityResult? deletionResult = await userManager.RemoveFromRolesAsync(user, roles);
            ThrowIfNotSuccessful(deletionResult, $"Could not clear roles from user '{userRecord.UserName}'");
        }

        // Now add the specific roles again
        if (userRecord.Roles is not null && userRecord.Roles.Count > 0)
        {
            IdentityResult? additionResult = await userManager.AddToRolesAsync(user, userRecord.Roles);
            ThrowIfNotSuccessful(additionResult, $"Could not add user '{userRecord.UserName}' to roles");
        }
    }

    /// <summary>
    /// Throws an appropriate error if the identity operation was not successful.
    /// </summary>
    /// <param name="result">The result of the identity operation.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="ApplicationException">Thrown if the identity operation was not successful.</exception>
    internal static void ThrowIfNotSuccessful(IdentityResult? result, string message)
    {
        if (result?.Succeeded != true)
        {
            throw new ApplicationException($"{message}: {result?.Errors.FirstOrDefault()?.Description}");
        }
    }

    internal record SeedUser(string UserName, string DisplayName, List<string>? Roles = null);
}
