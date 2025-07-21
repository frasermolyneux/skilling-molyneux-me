using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MX.Skilling.Web.Pages;

/// <summary>
/// Page model for the home page.
/// </summary>
/// <param name="logger">The logger instance.</param>
public sealed class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Handles GET requests to the home page.
    /// </summary>
    public async Task OnGetAsync()
    {
        _logger.LogInformation("Home page accessed");
        await Task.CompletedTask;
    }
}
