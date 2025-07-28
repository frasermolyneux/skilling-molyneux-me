using System.Security.Claims;

namespace MX.Skilling.Web.Services;

/// <summary>
/// Service for managing user roles through configuration.
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    /// Checks if a user is in the Admin role based on configured admin User Principal Names.
    /// </summary>
    /// <param name="user">The claims principal representing the user.</param>
    /// <returns>True if the user is an admin, false otherwise.</returns>
    Task<bool> IsUserInAdminRoleAsync(ClaimsPrincipal user);
}

/// <summary>
/// Implementation of user role service that checks against configured admin User Principal Names.
/// </summary>
/// <param name="logger">Logger for the service.</param>
/// <param name="configuration">Application configuration.</param>
public class UserRoleService(ILogger<UserRoleService> logger, IConfiguration configuration) : IUserRoleService
{
    /// <summary>
    /// Checks if a user is in the Admin role based on configured admin User Principal Names.
    /// </summary>
    /// <param name="user">The claims principal representing the user.</param>
    /// <returns>True if the user is an admin, false otherwise.</returns>
    public async Task<bool> IsUserInAdminRoleAsync(ClaimsPrincipal user)
    {
        if (!user.Identity?.IsAuthenticated == true)
        {
            return false;
        }

        var userPrincipalName = user.FindFirst("preferred_username")?.Value ??
                               user.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userPrincipalName))
        {
            logger.LogWarning("User Principal Name not found in claims");
            return false;
        }

        // Get admin User Principal Names from configuration
        // First try to get as array from configuration, then try as comma-separated string
        var adminUserPrincipalNames = configuration.GetSection("AdminUserPrincipalNames").Get<string[]>();

        if (adminUserPrincipalNames == null || adminUserPrincipalNames.Length == 0)
        {
            // Try to get as comma-separated string (from app settings)
            var adminUserPrincipalNamesString = configuration["AdminUserPrincipalNames"];
            if (!string.IsNullOrEmpty(adminUserPrincipalNamesString))
            {
                adminUserPrincipalNames = adminUserPrincipalNamesString
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(upn => upn.Trim())
                    .ToArray();
            }
            else
            {
                adminUserPrincipalNames = [];
            }
        }

        var isAdmin = adminUserPrincipalNames.Any(upn =>
            string.Equals(upn, userPrincipalName, StringComparison.OrdinalIgnoreCase));

        logger.LogInformation("User {UserPrincipalName} admin check result: {IsAdmin}", userPrincipalName, isAdmin);

        return await Task.FromResult(isAdmin);
    }
}
