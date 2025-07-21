using MX.Skilling.Web.UITests.Pages;
using MX.Skilling.Web.UITests.Support;
using static Microsoft.Playwright.Assertions;

namespace MX.Skilling.Web.UITests.Tests;

/// <summary>
/// UI tests for the home page functionality.
/// </summary>
[Trait("Category", "UI")]
public sealed class HomePageTests : PlaywrightTestBase
{
    /// <summary>
    /// Verifies that the home page displays the welcome heading when loaded.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenLoaded_DisplaysWelcomeHeading()
    {
        // Arrange
        var homePage = new HomePage(Page!);

        // Act
        await NavigateToAsync();
        await homePage.WaitForLoadAsync();

        // Assert
        await Expect(homePage.WelcomeHeading).ToBeVisibleAsync();
        await Expect(homePage.WelcomeHeading).ToContainTextAsync("Welcome");
    }

    /// <summary>
    /// Verifies that the home page displays the ASP.NET Core link when loaded.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenLoaded_DisplaysAspNetCoreLink()
    {
        // Arrange
        var homePage = new HomePage(Page!);

        // Act
        await NavigateToAsync();
        await homePage.WaitForLoadAsync();

        // Assert
        await Expect(homePage.AspNetCoreLink).ToBeVisibleAsync();
        await Expect(homePage.AspNetCoreLink).ToContainTextAsync("building Web apps with ASP.NET Core");
    }

    /// <summary>
    /// Verifies that the home page has the correct page title when loaded.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenLoaded_HasCorrectPageTitle()
    {
        // Arrange & Act
        await NavigateToAsync();

        // Assert
        await Expect(Page!).ToHaveTitleAsync(new Regex(".*Home page.*"));
    }

    /// <summary>
    /// Verifies that the ASP.NET Core link has the correct href attribute.
    /// </summary>
    [Fact]
    public async Task HomePage_AspNetCoreLink_HasCorrectHref()
    {
        // Arrange
        var homePage = new HomePage(Page!);

        // Act
        await NavigateToAsync();
        await homePage.WaitForLoadAsync();

        // Assert
        await Expect(homePage.AspNetCoreLink).ToHaveAttributeAsync("href", "https://learn.microsoft.com/aspnet/core");
    }

    /// <summary>
    /// Verifies that the main section has the correct automation ID attribute.
    /// </summary>
    [Fact]
    public async Task HomePage_MainSection_HasCorrectAutomationId()
    {
        // Arrange
        var homePage = new HomePage(Page!);

        // Act
        await NavigateToAsync();
        await homePage.WaitForLoadAsync();

        // Assert
        await Expect(homePage.MainSection).ToHaveAttributeAsync("data-automation-id", "home-main-section");
    }
}
