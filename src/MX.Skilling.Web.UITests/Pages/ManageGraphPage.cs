namespace MX.Skilling.Web.UITests.Pages;

/// <summary>
/// Page object model for the manage graph page of the application.
/// </summary>
public sealed class ManageGraphPage(IPage page)
{
    private readonly IPage _page = page ?? throw new ArgumentNullException(nameof(page));

    /// <summary>
    /// Selectors for elements on the manage graph page.
    /// </summary>
    public static class Selectors
    {
        /// <summary>
        /// Selector for the manage graph page heading element.
        /// </summary>
        public const string PageHeading = "[data-automation-id='manage-graph-heading']";

        /// <summary>
        /// Selector for the main section of the manage graph page.
        /// </summary>
        public const string MainSection = "[data-automation-id='manage-graph-main-section']";

        /// <summary>
        /// Selector for the manage graph description text.
        /// </summary>
        public const string Description = "[data-automation-id='manage-graph-description']";

        /// <summary>
        /// Selector for the user information card.
        /// </summary>
        public const string UserInfoCard = "[data-automation-id='user-info-card']";

        /// <summary>
        /// Selector for the user information heading.
        /// </summary>
        public const string UserInfoHeading = "[data-automation-id='user-info-heading']";

        /// <summary>
        /// Selector for the user display name.
        /// </summary>
        public const string UserDisplayName = "[data-automation-id='user-display-name']";

        /// <summary>
        /// Selector for the user principal name.
        /// </summary>
        public const string UserPrincipalName = "[data-automation-id='user-principal-name']";

        /// <summary>
        /// Selector for the user object ID.
        /// </summary>
        public const string UserObjectId = "[data-automation-id='user-object-id']";
    }

    /// <summary>
    /// Gets the page heading element.
    /// </summary>
    public ILocator PageHeading => _page.Locator(Selectors.PageHeading);

    /// <summary>
    /// Gets the main content section.
    /// </summary>
    public ILocator MainSection => _page.Locator(Selectors.MainSection);

    /// <summary>
    /// Gets the description paragraph.
    /// </summary>
    public ILocator Description => _page.Locator(Selectors.Description);

    /// <summary>
    /// Gets the user information card.
    /// </summary>
    public ILocator UserInfoCard => _page.Locator(Selectors.UserInfoCard);

    /// <summary>
    /// Gets the user information heading.
    /// </summary>
    public ILocator UserInfoHeading => _page.Locator(Selectors.UserInfoHeading);

    /// <summary>
    /// Gets the user display name element.
    /// </summary>
    public ILocator UserDisplayName => _page.Locator(Selectors.UserDisplayName);

    /// <summary>
    /// Gets the user principal name element.
    /// </summary>
    public ILocator UserPrincipalName => _page.Locator(Selectors.UserPrincipalName);

    /// <summary>
    /// Gets the user object ID element.
    /// </summary>
    public ILocator UserObjectId => _page.Locator(Selectors.UserObjectId);

    /// <summary>
    /// Navigates to the manage graph page.
    /// </summary>
    public async Task NavigateAsync(string baseUrl)
    {
        await _page.GotoAsync($"{baseUrl}/ManageGraph");
    }

    /// <summary>
    /// Waits for the manage graph page to be fully loaded.
    /// </summary>
    public async Task WaitForLoadAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await PageHeading.WaitForAsync();
    }
}
