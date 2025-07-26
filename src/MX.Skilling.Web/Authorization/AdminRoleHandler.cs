using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MX.Skilling.Web.Authorization;
using MX.Skilling.Web.Services;

namespace MX.Skilling.Web.Authorization;

/// <summary>
/// Authorization handler that checks if a user is in the Admin role.
/// </summary>
public class AdminRoleRequirement : IAuthorizationRequirement
{
}

/// <summary>
/// Authorization handler for Admin role requirement.
/// </summary>
/// <param name="userRoleService">Service for checking user roles.</param>
/// <param name="logger">Logger for the authorization handler.</param>
public class AdminRoleHandler(IUserRoleService userRoleService, ILogger<AdminRoleHandler> logger) : AuthorizationHandler<AdminRoleRequirement>
{
    /// <summary>
    /// Handles the authorization requirement by checking if the user is in the Admin role.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The admin role requirement.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminRoleRequirement requirement)
    {
        try
        {
            var isAdmin = await userRoleService.IsUserInAdminRoleAsync(context.User);

            if (isAdmin)
            {
                context.Succeed(requirement);
                logger.LogInformation("Admin authorization succeeded for user");
            }
            else
            {
                logger.LogWarning("Admin authorization failed for user");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during admin authorization check");
        }
    }
}
