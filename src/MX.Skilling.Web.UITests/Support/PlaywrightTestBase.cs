using System.Linq;

namespace MX.Skilling.Web.UITests.Support;

/// <summary>
/// Base class for Playwright UI tests providing common setup and configuration.
/// </summary>
[Trait("Category", "UI")]
public abstract class PlaywrightTestBase : IAsyncLifetime
{
    /// <summary>
    /// Gets the Playwright instance used for browser automation.
    /// </summary>
    protected IPlaywright? Playwright { get; private set; }

    /// <summary>
    /// Gets the browser instance used for testing.
    /// </summary>
    protected IBrowser? Browser { get; private set; }

    /// <summary>
    /// Gets the browser context for isolated testing.
    /// </summary>
    protected IBrowserContext? Context { get; private set; }

    /// <summary>
    /// Gets the page instance for web page interactions.
    /// </summary>
    protected IPage? Page { get; private set; }

    /// <summary>
    /// The base URL for the application under test.
    /// Can be configured via ASPNETCORE_URLS or WEB_APP_URL environment variables.
    /// Defaults to https://localhost:7053 (matches launchSettings.json).
    /// </summary>
    protected virtual string BaseUrl => GetBaseUrl();

    /// <summary>
    /// The browser type to use for tests. Defaults to Chromium.
    /// </summary>
    protected virtual string BrowserType => "chromium";

    /// <summary>
    /// Initializes the Playwright browser and page for testing.
    /// </summary>
    public virtual async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        var browserTypeLauncher = BrowserType.ToLower() switch
        {
            "firefox" => Playwright.Firefox,
            "webkit" => Playwright.Webkit,
            _ => Playwright.Chromium
        };

        Browser = await browserTypeLauncher.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = !IsDebugMode(),
            SlowMo = IsDebugMode() ? 100 : 0
        });

        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
            IgnoreHTTPSErrors = true
        });

        Page = await Context.NewPageAsync();
    }

    /// <summary>
    /// Cleans up browser resources after tests complete.
    /// </summary>
    public virtual async Task DisposeAsync()
    {
        if (Page != null)
        {
            await Page.CloseAsync();
        }

        if (Context != null)
        {
            await Context.CloseAsync();
        }

        if (Browser != null)
        {
            await Browser.CloseAsync();
        }

        Playwright?.Dispose();
    }

    /// <summary>
    /// Navigates to a page relative to the base URL.
    /// </summary>
    /// <param name="path">The relative path to navigate to.</param>
    protected async Task NavigateToAsync(string path = "/")
    {
        if (Page == null)
        {
            throw new InvalidOperationException("Page is not initialized. Ensure InitializeAsync has been called.");
        }

        var url = path.StartsWith('/') ? $"{BaseUrl}{path}" : $"{BaseUrl}/{path}";
        await Page.GotoAsync(url);
    }

    /// <summary>
    /// Determines if the test is running in debug mode.
    /// </summary>
    private static bool IsDebugMode()
    {
        return Environment.GetEnvironmentVariable("PLAYWRIGHT_DEBUG") == "1" ||
               System.Diagnostics.Debugger.IsAttached;
    }

    /// <summary>
    /// Gets the base URL for the application under test from environment variables or defaults.
    /// Checks WEB_APP_URL first, then ASPNETCORE_URLS, then falls back to default.
    /// </summary>
    private static string GetBaseUrl()
    {
        // Check for explicit test URL first
        var webAppUrl = Environment.GetEnvironmentVariable("WEB_APP_URL");
        if (!string.IsNullOrEmpty(webAppUrl))
        {
            return webAppUrl.TrimEnd('/');
        }

        // Check ASPNETCORE_URLS (used by the web app)
        var aspNetCoreUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
        if (!string.IsNullOrEmpty(aspNetCoreUrls))
        {
            // ASPNETCORE_URLS can be multiple URLs separated by semicolons
            // Prefer HTTPS if available, otherwise use the first URL
            var urls = aspNetCoreUrls.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var httpsUrl = urls.FirstOrDefault(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
            if (httpsUrl != null)
            {
                return httpsUrl.TrimEnd('/');
            }

            return urls[0].TrimEnd('/');
        }

        // Default to the HTTPS URL from launchSettings.json
        return "https://localhost:7053";
    }
}
