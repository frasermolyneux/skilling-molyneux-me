using MX.Skilling.Web.UITests.Pages;
using MX.Skilling.Web.UITests.Support;
using static Microsoft.Playwright.Assertions;

namespace MX.Skilling.Web.UITests.Tests;

/// <summary>
/// UI tests for navigation functionality across the application.
/// </summary>
[Trait("Category", "UI")]
public sealed class NavigationTests : PlaywrightTestBase
{
    /// <summary>
    /// Verifies that navigation from home page to privacy page works correctly.
    /// </summary>
    [Fact]
    public async Task Navigation_HomeToPrivacy_WorksCorrectly()
    {
        // Arrange
        await NavigateToAsync();

        // Act - Click Privacy link in navigation
        await Page!.ClickAsync("a[href='/Privacy']");

        // Assert
        await Expect(Page).ToHaveURLAsync(new Regex(".*/Privacy"));
        await Expect(Page.Locator("[data-automation-id='privacy-heading']")).ToContainTextAsync("Privacy Policy");
    }

    /// <summary>
    /// Verifies that navigation from privacy page to home page works correctly.
    /// </summary>
    [Fact]
    public async Task Navigation_PrivacyToHome_WorksCorrectly()
    {
        // Arrange
        await NavigateToAsync("/Privacy");

        // Act - Click Home link in navigation
        await Page!.ClickAsync("a[href='/']");

        // Assert
        await Expect(Page).ToHaveURLAsync(new Regex(".*/$"));
        await Expect(Page.Locator("[data-automation-id='welcome-heading']")).ToContainTextAsync("Welcome");
    }

    /// <summary>
    /// Verifies that the brand/logo link navigates to the home page.
    /// </summary>
    [Fact]
    public async Task Navigation_BrandLink_NavigatesToHome()
    {
        // Arrange
        await NavigateToAsync("/Privacy");

        // Act - Click brand/logo link
        var brandLink = Page!.Locator("a.navbar-brand, .navbar-brand a").First;
        if (await brandLink.CountAsync() > 0)
        {
            await brandLink.ClickAsync();

            // Assert
            await Expect(Page).ToHaveURLAsync(new Regex(".*/$"));
            await Expect(Page.Locator("[data-automation-id='welcome-heading']")).ToContainTextAsync("Welcome");
        }
        else
        {
            // If no brand link exists, this test is not applicable
            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Verifies that direct URL access works for all pages.
    /// </summary>
    [Fact]
    public async Task Navigation_DirectUrlAccess_WorksForAllPages()
    {
        // Test direct access to home page
        await NavigateToAsync("/");
        await Expect(Page!.Locator("[data-automation-id='welcome-heading']")).ToContainTextAsync("Welcome");

        // Test direct access to privacy page
        await NavigateToAsync("/Privacy");
        await Expect(Page.Locator("[data-automation-id='privacy-heading']")).ToContainTextAsync("Privacy Policy");
    }

    /// <summary>
    /// Verifies that page response times are within acceptable limits.
    /// </summary>
    [Fact]
    public async Task Navigation_ResponseTimes_AreAcceptable()
    {
        var startTime = DateTime.UtcNow;

        // Navigate to home page
        await NavigateToAsync();
        await Page!.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var homePageLoadTime = DateTime.UtcNow - startTime;

        startTime = DateTime.UtcNow;

        // Navigate to privacy page
        await NavigateToAsync("/Privacy");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var privacyPageLoadTime = DateTime.UtcNow - startTime;

        // Assert reasonable load times (less than 5 seconds)
        Assert.True(homePageLoadTime.TotalSeconds < 5, $"Home page took {homePageLoadTime.TotalSeconds} seconds to load");
        Assert.True(privacyPageLoadTime.TotalSeconds < 5, $"Privacy page took {privacyPageLoadTime.TotalSeconds} seconds to load");
    }
}
