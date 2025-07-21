using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MX.Skilling.Web.Pages;

/// <summary>
/// Page model for the privacy policy page.
/// </summary>
/// <param name="logger">The logger instance.</param>
public sealed class PrivacyModel(ILogger<PrivacyModel> logger) : PageModel
{
    private readonly ILogger<PrivacyModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Handles GET requests to the privacy policy page.
    /// </summary>
    public async Task OnGetAsync()
    {
        _logger.LogInformation("Privacy policy page accessed");
        await Task.CompletedTask;
    }
}
