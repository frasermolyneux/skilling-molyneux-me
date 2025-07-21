namespace MX.Skilling.Web.UITests.Pages;

/// <summary>
/// Page object model for the privacy page of the application.
/// </summary>
public sealed class PrivacyPage(IPage page)
{
    private readonly IPage _page = page ?? throw new ArgumentNullException(nameof(page));

    /// <summary>
    /// Selectors for elements on the privacy page.
    /// </summary>
    public static class Selectors
    {
        /// <summary>
        /// Selector for the privacy page heading element.
        /// </summary>
        public const string PageHeading = "[data-automation-id='privacy-heading']";

        /// <summary>
        /// Selector for the main content section of the privacy page.
        /// </summary>
        public const string PageContent = "[data-automation-id='privacy-main-section']";

        /// <summary>
        /// Selector for the privacy description text.
        /// </summary>
        public const string PrivacyDescription = "[data-automation-id='privacy-description']";
    }

    /// <summary>
    /// Gets the page heading element.
    /// </summary>
    public ILocator PageHeading => _page.Locator(Selectors.PageHeading);

    /// <summary>
    /// Gets the main content area.
    /// </summary>
    public ILocator PageContent => _page.Locator(Selectors.PageContent);

    /// <summary>
    /// Gets the privacy description paragraph.
    /// </summary>
    public ILocator PrivacyDescription => _page.Locator(Selectors.PrivacyDescription);

    /// <summary>
    /// Navigates to the privacy page.
    /// </summary>
    public async Task NavigateAsync(string baseUrl)
    {
        await _page.GotoAsync($"{baseUrl}/Privacy");
    }

    /// <summary>
    /// Waits for the privacy page to be fully loaded.
    /// </summary>
    public async Task WaitForLoadAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await PageHeading.WaitForAsync();
    }
}
