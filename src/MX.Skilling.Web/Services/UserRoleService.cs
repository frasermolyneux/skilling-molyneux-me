using System.Security.Claims;

namespace MX.Skilling.Web.Services;

/// <summary>
/// Service for managing user roles through configuration.
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    /// Checks if a user is in the Admin role based on configured admin emails.
    /// </summary>
    /// <param name="user">The claims principal representing the user.</param>
    /// <returns>True if the user is an admin, false otherwise.</returns>
    Task<bool> IsUserInAdminRoleAsync(ClaimsPrincipal user);
}

/// <summary>
/// Implementation of user role service that checks against configured admin emails.
/// </summary>
/// <param name="logger">Logger for the service.</param>
/// <param name="configuration">Application configuration.</param>
public class UserRoleService(ILogger<UserRoleService> logger, IConfiguration configuration) : IUserRoleService
{
    /// <summary>
    /// Checks if a user is in the Admin role based on configured admin emails.
    /// </summary>
    /// <param name="user">The claims principal representing the user.</param>
    /// <returns>True if the user is an admin, false otherwise.</returns>
    public async Task<bool> IsUserInAdminRoleAsync(ClaimsPrincipal user)
    {
        if (!user.Identity?.IsAuthenticated == true)
        {
            return false;
        }

        var userEmail = user.FindFirst(ClaimTypes.Email)?.Value ??
                       user.FindFirst("preferred_username")?.Value;

        if (string.IsNullOrEmpty(userEmail))
        {
            logger.LogWarning("User email not found in claims");
            return false;
        }

        // Get admin emails from configuration
        // First try to get as array from configuration, then try as comma-separated string
        var adminEmails = configuration.GetSection("AdminEmails").Get<string[]>();

        if (adminEmails == null || adminEmails.Length == 0)
        {
            // Try to get as comma-separated string (from app settings)
            var adminEmailsString = configuration["AdminEmails"];
            if (!string.IsNullOrEmpty(adminEmailsString))
            {
                adminEmails = adminEmailsString
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(email => email.Trim())
                    .ToArray();
            }
            else
            {
                adminEmails = [];
            }
        }

        var isAdmin = adminEmails.Any(email =>
            string.Equals(email, userEmail, StringComparison.OrdinalIgnoreCase));

        logger.LogInformation("User {UserEmail} admin check result: {IsAdmin}", userEmail, isAdmin);

        return await Task.FromResult(isAdmin);
    }
}
