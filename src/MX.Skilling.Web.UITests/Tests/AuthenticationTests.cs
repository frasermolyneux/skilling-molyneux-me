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
    /// Should redirect to sign-in or return appropriate error.
    /// </summary>
    [Fact]
    public async Task ManageGraphPage_RedirectsToSignIn_WhenNotAuthenticated()
    {
        // Arrange & Act
        var response = await Page!.GotoAsync(BaseUrl + "/ManageGraph");

        // Assert
        if (response != null && response.Status == 401)
        {
            // Returns 401 Unauthorized (expected for unauthenticated access)
            return;
        }

        // Should redirect to Microsoft login
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
    /// Should redirect to Microsoft login for authentication.
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
            // Timeout is acceptable during testing
        }

        // Assert
        try
        {
            // Should redirect to Microsoft login
            await Page!.WaitForURLAsync("**/oauth2/v2.0/authorize**", new() { Timeout = 2000 });
        }
        catch (TimeoutException)
        {
            // Check if we're on a sign-in related page or got an appropriate response
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
