namespace MX.Skilling.Web.UITests.Pages;

/// <summary>
/// Page object model for shared navigation elements across the application.
/// </summary>
public sealed class NavigationPage(IPage page)
{
    private readonly IPage _page = page ?? throw new ArgumentNullException(nameof(page));

    /// <summary>
    /// Selectors for navigation elements.
    /// </summary>
    public static class Selectors
    {
        /// <summary>
        /// Selector for the navbar brand link.
        /// </summary>
        public const string NavbarBrand = "[data-automation-id='navbar-brand-link']";

        /// <summary>
        /// Selector for the navbar toggle button.
        /// </summary>
        public const string NavbarToggle = "[data-automation-id='navbar-toggle-button']";

        /// <summary>
        /// Selector for the home navigation link.
        /// </summary>
        public const string HomeNavLink = "[data-automation-id='home-nav-link']";

        /// <summary>
        /// Selector for the privacy navigation link.
        /// </summary>
        public const string PrivacyNavLink = "[data-automation-id='privacy-nav-link']";

        /// <summary>
        /// Selector for the manage graph navigation link.
        /// </summary>
        public const string ManageGraphNavLink = "[data-automation-id='manage-graph-nav-link']";

        /// <summary>
        /// Selector for the sign in link.
        /// </summary>
        public const string SignInLink = "[data-automation-id='sign-in-link']";

        /// <summary>
        /// Selector for the user dropdown toggle.
        /// </summary>
        public const string UserDropdownToggle = "[data-automation-id='user-dropdown-toggle']";

        /// <summary>
        /// Selector for the sign out link.
        /// </summary>
        public const string SignOutLink = "[data-automation-id='sign-out-link']";

        /// <summary>
        /// Selector for the footer privacy link.
        /// </summary>
        public const string FooterPrivacyLink = "[data-automation-id='footer-privacy-link']";
    }

    /// <summary>
    /// Gets the navbar brand link.
    /// </summary>
    public ILocator NavbarBrand => _page.Locator(Selectors.NavbarBrand);

    /// <summary>
    /// Gets the navbar toggle button.
    /// </summary>
    public ILocator NavbarToggle => _page.Locator(Selectors.NavbarToggle);

    /// <summary>
    /// Gets the home navigation link.
    /// </summary>
    public ILocator HomeNavLink => _page.Locator(Selectors.HomeNavLink);

    /// <summary>
    /// Gets the privacy navigation link.
    /// </summary>
    public ILocator PrivacyNavLink => _page.Locator(Selectors.PrivacyNavLink);

    /// <summary>
    /// Gets the manage graph navigation link.
    /// </summary>
    public ILocator ManageGraphNavLink => _page.Locator(Selectors.ManageGraphNavLink);

    /// <summary>
    /// Gets the sign in link.
    /// </summary>
    public ILocator SignInLink => _page.Locator(Selectors.SignInLink);

    /// <summary>
    /// Gets the user dropdown toggle.
    /// </summary>
    public ILocator UserDropdownToggle => _page.Locator(Selectors.UserDropdownToggle);

    /// <summary>
    /// Gets the sign out link.
    /// </summary>
    public ILocator SignOutLink => _page.Locator(Selectors.SignOutLink);

    /// <summary>
    /// Gets the footer privacy link.
    /// </summary>
    public ILocator FooterPrivacyLink => _page.Locator(Selectors.FooterPrivacyLink);

    /// <summary>
    /// Clicks the home navigation link.
    /// </summary>
    public async Task ClickHomeAsync()
    {
        await HomeNavLink.ClickAsync();
    }

    /// <summary>
    /// Clicks the privacy navigation link.
    /// </summary>
    public async Task ClickPrivacyAsync()
    {
        await PrivacyNavLink.ClickAsync();
    }

    /// <summary>
    /// Clicks the manage graph navigation link.
    /// </summary>
    public async Task ClickManageGraphAsync()
    {
        await ManageGraphNavLink.ClickAsync();
    }

    /// <summary>
    /// Clicks the sign in link.
    /// </summary>
    public async Task ClickSignInAsync()
    {
        await SignInLink.ClickAsync();
    }

    /// <summary>
    /// Clicks the sign out link.
    /// </summary>
    public async Task ClickSignOutAsync()
    {
        // First open the user dropdown menu
        await UserDropdownToggle.ClickAsync();

        // Then click the sign out link
        await SignOutLink.ClickAsync();
    }

    /// <summary>
    /// Clicks the navbar brand to navigate to home.
    /// </summary>
    public async Task ClickBrandAsync()
    {
        await NavbarBrand.ClickAsync();
    }
}
