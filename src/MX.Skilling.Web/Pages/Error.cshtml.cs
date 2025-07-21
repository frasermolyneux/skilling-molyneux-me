using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MX.Skilling.Web.Pages;

/// <summary>
/// Page model for displaying application errors.
/// </summary>
/// <param name="logger">The logger instance.</param>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public sealed class ErrorModel(ILogger<ErrorModel> logger) : PageModel
{
    private readonly ILogger<ErrorModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Gets or sets the request ID for error tracking.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether to show the request ID to the user.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    /// <summary>
    /// Handles GET requests to the error page and sets the request ID.
    /// </summary>
    public async Task OnGetAsync()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Error page accessed with RequestId: {RequestId}", RequestId);
        await Task.CompletedTask;
    }
}
