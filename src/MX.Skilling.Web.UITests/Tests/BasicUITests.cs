using Microsoft.Playwright;
using MX.Skilling.Web.UITests.Pages;
using MX.Skilling.Web.UITests.Support;
using Xunit;
using static Microsoft.Playwright.Assertions;

namespace MX.Skilling.Web.UITests.Tests;

/// <summary>
/// Basic UI tests that don't require authentication.
/// </summary>
[Trait("Category", "UI")]
public sealed class BasicUITests : PlaywrightTestBase
{
    /// <summary>
    /// Tests that the home page loads correctly.
    /// </summary>
    [Fact]
    public async Task HomePage_LoadsSuccessfully()
    {
        // Arrange
        var homePage = new HomePage(Page!);
        var navigation = new NavigationPage(Page!);

        // Act
        await NavigateToAsync("/");

        // Assert
        await Expect(homePage.WelcomeHeading).ToContainTextAsync("Welcome");
        await Expect(navigation.PrivacyNavLink).ToBeVisibleAsync();
    }

    /// <summary>
    /// Tests that the privacy page loads correctly.
    /// </summary>
    [Fact]
    public async Task PrivacyPage_LoadsSuccessfully()
    {
        // Arrange
        var privacyPage = new PrivacyPage(Page!);

        // Act
        await NavigateToAsync("/Privacy");

        // Assert
        await Expect(privacyPage.PageHeading).ToContainTextAsync("Privacy Policy");
    }

    /// <summary>
    /// Tests navigation between public pages.
    /// </summary>
    [Fact]
    public async Task Navigation_WorksBetweenPublicPages()
    {
        // Arrange
        var homePage = new HomePage(Page!);
        var privacyPage = new PrivacyPage(Page!);
        var navigation = new NavigationPage(Page!);

        // Start at home page
        await NavigateToAsync("/");
        await Expect(homePage.WelcomeHeading).ToContainTextAsync("Welcome");

        // Navigate to privacy page
        await navigation.ClickPrivacyAsync();
        await Expect(privacyPage.PageHeading).ToContainTextAsync("Privacy Policy");

        // Navigate back to home
        await navigation.ClickHomeAsync();
        await Expect(homePage.WelcomeHeading).ToContainTextAsync("Welcome");
    }

    /// <summary>
    /// Tests that unauthenticated users see sign-in option.
    /// </summary>
    [Fact]
    public async Task UnauthenticatedUser_SeesSignInOption()
    {
        // Arrange
        var navigation = new NavigationPage(Page!);

        // Act
        await NavigateToAsync("/");

        // Assert - Should see sign-in link
        await Expect(navigation.SignInLink).ToBeVisibleAsync();
    }
}
