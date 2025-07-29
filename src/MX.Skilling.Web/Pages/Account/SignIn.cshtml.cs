using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        // In UITest mode, JWT Bearer is used and challenges should return 401
        // In production mode, OpenIdConnect is used for sign-in redirects
        var enableUITesting = HttpContext.RequestServices.GetRequiredService<IConfiguration>()
            .GetValue<bool>("UITesting:Enabled");

        if (enableUITesting)
        {
            // For UI testing with JWT Bearer, return 401 Unauthorized
            return Unauthorized();
        }

        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUrl },
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}
