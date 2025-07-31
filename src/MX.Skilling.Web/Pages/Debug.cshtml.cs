using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MX.Skilling.Web.Authorization;
using MX.Skilling.Web.Services;

namespace MX.Skilling.Web.Pages;

/// <summary>
/// Debug page for troubleshooting authentication and authorization issues.
/// </summary>
/// <param name="userRoleService">Service for checking user roles.</param>
/// <param name="authorizationService">Service for authorization checks.</param>
public class DebugModel(IUserRoleService userRoleService, IAuthorizationService authorizationService) : PageModel
{
    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    public bool IsAuthenticated { get; private set; }

    /// <summary>
    /// Gets the authentication type of the current user.
    /// </summary>
    public string? AuthenticationType { get; private set; }

    /// <summary>
    /// Gets the name of the current user.
    /// </summary>
    public string? UserName { get; private set; }

    /// <summary>
    /// Gets the name claim type for the current user identity.
    /// </summary>
    public string? NameClaimType { get; private set; }

    /// <summary>
    /// Gets the claims associated with the current user.
    /// </summary>
    public List<Claim> Claims { get; private set; } = new();

    /// <summary>
    /// Gets a value indicating whether the current user is an admin.
    /// </summary>
    public bool IsAdmin { get; private set; }

    /// <summary>
    /// Gets the result of the admin policy authorization check.
    /// </summary>
    public string AdminPolicyResult { get; private set; } = "Unknown";

    /// <summary>
    /// Handles GET requests to populate debug information.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task OnGetAsync()
    {
        IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
        AuthenticationType = User.Identity?.AuthenticationType;
        UserName = User.Identity?.Name;

        // Handle ClaimsIdentity-specific properties safely
        if (User.Identity is ClaimsIdentity claimsIdentity)
        {
            NameClaimType = claimsIdentity.NameClaimType;
        }

        if (User.Claims.Any())
        {
            Claims = User.Claims.ToList();
        }

        if (IsAuthenticated)
        {
            IsAdmin = await userRoleService.IsUserInAdminRoleAsync(User);

            var authResult = await authorizationService.AuthorizeAsync(User, AuthorizationConstants.AdminPolicy);
            AdminPolicyResult = authResult.Succeeded ? "Succeeded" : "Failed";
        }
    }
}
