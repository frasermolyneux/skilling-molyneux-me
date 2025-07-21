namespace MX.Skilling.Web.UITests.Pages;

/// <summary>
/// Page object model for the home page of the application.
/// </summary>
public sealed class HomePage(IPage page)
{
    private readonly IPage _page = page ?? throw new ArgumentNullException(nameof(page));

    /// <summary>
    /// Selectors for elements on the home page.
    /// </summary>
    public static class Selectors
    {
        /// <summary>
        /// Selector for the welcome heading element.
        /// </summary>
        public const string WelcomeHeading = "[data-automation-id='welcome-heading']";

        /// <summary>
        /// Selector for the ASP.NET Core documentation link.
        /// </summary>
        public const string AspNetCoreLink = "[data-automation-id='aspnet-core-link']";

        /// <summary>
        /// Selector for the main section of the home page.
        /// </summary>
        public const string MainSection = "[data-automation-id='home-main-section']";

        /// <summary>
        /// Selector for the welcome description text.
        /// </summary>
        public const string WelcomeDescription = "[data-automation-id='welcome-description']";
    }

    /// <summary>
    /// Gets the welcome heading element.
    /// </summary>
    public ILocator WelcomeHeading => _page.Locator(Selectors.WelcomeHeading);

    /// <summary>
    /// Gets the ASP.NET Core documentation link.
    /// </summary>
    public ILocator AspNetCoreLink => _page.Locator(Selectors.AspNetCoreLink);

    /// <summary>
    /// Gets the main content section.
    /// </summary>
    public ILocator MainSection => _page.Locator(Selectors.MainSection);

    /// <summary>
    /// Gets the welcome description paragraph.
    /// </summary>
    public ILocator WelcomeDescription => _page.Locator(Selectors.WelcomeDescription);

    /// <summary>
    /// Navigates to the home page.
    /// </summary>
    public async Task NavigateAsync(string baseUrl)
    {
        await _page.GotoAsync(baseUrl);
    }

    /// <summary>
    /// Clicks the ASP.NET Core documentation link.
    /// </summary>
    public async Task ClickAspNetCoreLinkAsync()
    {
        await AspNetCoreLink.ClickAsync();
    }

    /// <summary>
    /// Waits for the page to be fully loaded.
    /// </summary>
    public async Task WaitForLoadAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await WelcomeHeading.WaitForAsync();
    }
}
