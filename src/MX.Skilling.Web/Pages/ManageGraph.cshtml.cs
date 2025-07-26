using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MX.Skilling.Web.Authorization;

namespace MX.Skilling.Web.Pages;

/// <summary>
/// Page model for the Manage Graph page, accessible only to Admin users.
/// </summary>
/// <param name="logger">The logger instance.</param>
[Authorize(Policy = AuthorizationConstants.AdminPolicy)]
public class ManageGraphModel(ILogger<ManageGraphModel> logger) : PageModel
{
    /// <summary>
    /// Handles GET requests to the Manage Graph page.
    /// </summary>
    public void OnGet()
    {
        logger.LogInformation("ManageGraph page accessed by user: {UserName}", User.Identity?.Name);
    }
}
