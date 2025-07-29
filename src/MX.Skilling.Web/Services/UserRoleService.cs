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

/// <summary>
/// UI Test implementation of user role service that logs all claims to help debug.
/// </summary>
/// <param name="logger">Logger for the service.</param>
public class UITestUserRoleService(ILogger<UITestUserRoleService> logger) : IUserRoleService
{
    /// <summary>
    /// For UI testing, check token audience to determine admin role.
    /// Admin tokens use specific client ID, User tokens use different client ID.
    /// </summary>
    /// <param name="user">The claims principal representing the user.</param>
    /// <returns>True if the token is from admin app registration, false otherwise.</returns>
    public async Task<bool> IsUserInAdminRoleAsync(ClaimsPrincipal user)
    {
        if (!user.Identity?.IsAuthenticated == true)
        {
            return false;
        }

        // Check the audience claim to identify admin vs user tokens
        var audience = user.FindFirst("aud")?.Value;
        if (!string.IsNullOrEmpty(audience))
        {
            // Admin app registration has audience: api://0074007f-8657-44d6-9e1a-f7a01c1a8e78
            // User app registration has audience: api://1024cc02-a8ce-40c8-b08a-311244980223
            var adminClientId = "0074007f-8657-44d6-9e1a-f7a01c1a8e78";
            var isAdmin = audience.Contains(adminClientId, StringComparison.OrdinalIgnoreCase);

            logger.LogInformation("UI Test mode: Audience {Audience} admin check result: {IsAdmin}", audience, isAdmin);
            return await Task.FromResult(isAdmin);
        }

        // Fallback: return false (not admin)
        logger.LogWarning("UI Test mode: Could not determine admin status from claims");
        return await Task.FromResult(false);
    }
}
