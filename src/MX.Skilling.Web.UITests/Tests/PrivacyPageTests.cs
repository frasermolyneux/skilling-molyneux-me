using MX.Skilling.Web.UITests.Pages;
using MX.Skilling.Web.UITests.Support;
using static Microsoft.Playwright.Assertions;

namespace MX.Skilling.Web.UITests.Tests;

/// <summary>
/// UI tests for the privacy page functionality.
/// </summary>
[Trait("Category", "UI")]
public sealed class PrivacyPageTests : PlaywrightTestBase
{
    /// <summary>
    /// Verifies that the privacy page displays the correct heading when loaded.
    /// </summary>
    [Fact]
    public async Task PrivacyPage_WhenLoaded_DisplaysCorrectHeading()
    {
        // Arrange
        var privacyPage = new PrivacyPage(Page!);

        // Act
        await NavigateToAsync("/Privacy");
        await privacyPage.WaitForLoadAsync();

        // Assert
        await Expect(privacyPage.PageHeading).ToBeVisibleAsync();
        await Expect(privacyPage.PageHeading).ToContainTextAsync("Privacy Policy");
    }

    /// <summary>
    /// Verifies that the privacy page has the correct page title when loaded.
    /// </summary>
    [Fact]
    public async Task PrivacyPage_WhenLoaded_HasCorrectPageTitle()
    {
        // Arrange & Act
        await NavigateToAsync("/Privacy");

        // Assert
        await Expect(Page!).ToHaveTitleAsync(new Regex(".*Privacy Policy.*"));
    }

    /// <summary>
    /// Verifies that the privacy page displays the privacy content when loaded.
    /// </summary>
    [Fact]
    public async Task PrivacyPage_WhenLoaded_DisplaysPrivacyContent()
    {
        // Arrange
        var privacyPage = new PrivacyPage(Page!);

        // Act
        await NavigateToAsync("/Privacy");
        await privacyPage.WaitForLoadAsync();

        // Assert
        await Expect(privacyPage.PrivacyDescription).ToBeVisibleAsync();
        await Expect(privacyPage.PrivacyDescription).ToContainTextAsync("Use this page to detail your site's privacy policy");
    }

    /// <summary>
    /// Verifies that the heading element has the correct automation ID attribute.
    /// </summary>
    [Fact]
    public async Task PrivacyPage_HeadingElement_HasCorrectAutomationId()
    {
        // Arrange
        var privacyPage = new PrivacyPage(Page!);

        // Act
        await NavigateToAsync("/Privacy");
        await privacyPage.WaitForLoadAsync();

        // Assert
        await Expect(privacyPage.PageHeading).ToHaveAttributeAsync("data-automation-id", "privacy-heading");
    }
}
