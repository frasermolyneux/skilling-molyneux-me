using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MX.Skilling.Web.Pages.Account;

/// <summary>
/// Page model for handling user sign-out.
/// </summary>
public class SignOutModel : PageModel
{
    /// <summary>
    /// Handles GET requests to initiate sign-out.
    /// </summary>
    /// <returns>A sign-out result that clears authentication cookies and redirects to Azure AD sign-out.</returns>
    public async Task<IActionResult> OnGetAsync()
    {
        // Clear local authentication cookies and redirect to Azure AD sign-out
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                RedirectUri = Url.Page("/Index")
            });

        return new EmptyResult();
    }
}
