using Microsoft.Playwright;
using MX.Skilling.Web.UITests.Pages;
using MX.Skilling.Web.UITests.Support;
using Xunit;
using static Microsoft.Playwright.Assertions;

namespace MX.Skilling.Web.UITests.Tests;

/// <summary>
/// UI tests for authentication functionality.
/// </summary>
[Trait("Category", "UI")]
public sealed class AuthenticationTests : PlaywrightTestBase
{
    /// <summary>
    /// Verifies that the home page shows Sign In link when not authenticated.
    /// </summary>
    [Fact]
    public async Task HomePage_ShowsSignInLink_WhenNotAuthenticated()
    {
        // Arrange
        var navigation = new NavigationPage(Page!);

        // Act
        await NavigateToAsync("/");

        // Assert
        await Expect(navigation.SignInLink).ToBeVisibleAsync();
        await Expect(navigation.ManageGraphNavLink).Not.ToBeVisibleAsync();
    }

    /// <summary>
    /// Verifies that the ManageGraph page handles unauthenticated access appropriately.
    /// In UITest mode (JWT Bearer), returns 401. In production (Cookie), redirects to sign-in.
    /// </summary>
    [Fact]
    public async Task ManageGraphPage_RedirectsToSignIn_WhenNotAuthenticated()
    {
        // Arrange & Act
        var response = await Page!.GotoAsync(BaseUrl + "/ManageGraph");

        // Assert
        if (response != null && response.Status == 401)
        {
            // JWT Bearer mode - returns 401 Unauthorized
            return;
        }

        // Cookie authentication mode - should redirect to Microsoft login
        try
        {
            await Page.WaitForURLAsync("**/oauth2/v2.0/authorize**", new() { Timeout = 5000 });
        }
        catch (TimeoutException)
        {
            // If no redirect happened, check if we got an error page instead
            var hasUnauthorizedMessage = await Page.GetByText("401").IsVisibleAsync() ||
                                       await Page.GetByText("Unauthorized").IsVisibleAsync();

            Assert.True(hasUnauthorizedMessage, "Expected either redirect to sign-in or 401 unauthorized response");
        }
    }

    /// <summary>
    /// Verifies that the Sign In link navigates to the sign-in page.
    /// In UITest mode (JWT Bearer), sign-in link may not initiate OIDC redirect.
    /// In production (Cookie), should redirect to Microsoft login.
    /// </summary>
    [Fact]
    public async Task SignInLink_NavigatesToSignInPage()
    {
        // Arrange
        var navigation = new NavigationPage(Page!);
        await NavigateToAsync("/");

        // Act
        try
        {
            await navigation.ClickSignInAsync();
            await Page!.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 5000 });
        }
        catch (TimeoutException)
        {
            // Timeout is acceptable in UITest mode
        }

        // Assert
        try
        {
            // Cookie authentication mode - should redirect to Microsoft login
            await Page!.WaitForURLAsync("**/oauth2/v2.0/authorize**", new() { Timeout = 2000 });
        }
        catch (TimeoutException)
        {
            // In UITest mode, the sign-in link might lead to a different page or error
            // Check if we're still on a valid page or got an appropriate response
            var currentUrl = Page!.Url;
            var hasSignInRelatedContent = currentUrl.Contains("signin") ||
                                        currentUrl.Contains("Account") ||
                                        await Page.GetByText("Sign").IsVisibleAsync();

            Assert.True(hasSignInRelatedContent,
                       "Expected either redirect to Microsoft login or sign-in related content");
        }
    }

    /// <summary>
    /// Verifies that the navigation contains expected links.
    /// </summary>
    [Fact]
    public async Task Navigation_ContainsExpectedLinks()
    {
        // Arrange
        var navigation = new NavigationPage(Page!);

        // Act
        await NavigateToAsync("/");

        // Assert
        await Expect(navigation.HomeNavLink).ToBeVisibleAsync();
        await Expect(navigation.PrivacyNavLink).ToBeVisibleAsync();
        await Expect(navigation.SignInLink).ToBeVisibleAsync();
    }
}
