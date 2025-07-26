using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MX.Skilling.Web.Pages.Account;

/// <summary>
/// Page model for initiating sign-in process.
/// </summary>
public class SignInModel : PageModel
{
    /// <summary>
    /// Handles GET requests to initiate sign-in.
    /// </summary>
    /// <param name="returnUrl">The URL to return to after successful authentication.</param>
    /// <returns>A challenge result to initiate authentication.</returns>
    public IActionResult OnGet(string? returnUrl = null)
    {
        var redirectUrl = returnUrl ?? Url.Page("/Index");
        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUrl },
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}
