using Microsoft.Playwright;
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
        // Arrange & Act
        await NavigateToAsync("/");

        // Assert
        await Expect(Page!.GetByRole(AriaRole.Link, new() { Name = "Sign In" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Manage Graph" })).Not.ToBeVisibleAsync();
    }

    /// <summary>
    /// Verifies that the ManageGraph page redirects to sign-in when not authenticated.
    /// </summary>
    [Fact]
    public async Task ManageGraphPage_RedirectsToSignIn_WhenNotAuthenticated()
    {
        // Arrange & Act
        await NavigateToAsync("/ManageGraph");

        // Assert
        // The page should redirect to the sign-in flow
        await Page!.WaitForURLAsync("**/signin-oidc**");
    }

    /// <summary>
    /// Verifies that the Sign In link navigates to the sign-in page.
    /// </summary>
    [Fact]
    public async Task SignInLink_NavigatesToSignInPage()
    {
        // Arrange
        await NavigateToAsync("/");

        // Act
        await Page!.GetByRole(AriaRole.Link, new() { Name = "Sign In" }).ClickAsync();

        // Assert
        // Should be redirected to Azure AD or local sign-in handling
        await Page.WaitForURLAsync("**/Account/SignIn**");
    }

    /// <summary>
    /// Verifies that the navigation contains expected links.
    /// </summary>
    [Fact]
    public async Task Navigation_ContainsExpectedLinks()
    {
        // Arrange & Act
        await NavigateToAsync("/");

        // Assert
        await Expect(Page!.GetByRole(AriaRole.Link, new() { Name = "Home" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Privacy" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Sign In" })).ToBeVisibleAsync();
    }
}
