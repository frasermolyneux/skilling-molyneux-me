using Microsoft.AspNetCore.Mvc.RazorPages;
using MX.Skilling.Web.Pages;

namespace MX.Skilling.Web.Tests.Pages;

/// <summary>
/// Unit tests for the <see cref="PrivacyModel"/> page model.
/// </summary>
[Trait("Category", "Unit")]
public class PrivacyModelTests
{
    /// <summary>
    /// Verifies that OnGet method completes successfully without throwing exceptions.
    /// </summary>
    [Fact]
    public void OnGet_ShouldCompleteSuccessfully()
    {
        // Arrange
        var pageModel = new PrivacyModel();

        // Act & Assert - Should not throw
        pageModel.OnGet();
    }

    /// <summary>
    /// Verifies that PrivacyModel inherits from PageModel.
    /// </summary>
    [Fact]
    public void PrivacyModel_ShouldInheritFromPageModel()
    {
        // Arrange & Act
        var pageModel = new PrivacyModel();

        // Assert
        Assert.IsAssignableFrom<PageModel>(pageModel);
    }

    /// <summary>
    /// Verifies that PrivacyModel can be instantiated without dependencies.
    /// </summary>
    [Fact]
    public void PrivacyModel_CanBeInstantiatedWithoutDependencies()
    {
        // Act & Assert - Should not throw
        var pageModel = new PrivacyModel();
        Assert.NotNull(pageModel);
    }

    /// <summary>
    /// Verifies that OnGet method can be called multiple times safely.
    /// </summary>
    [Fact]
    public void OnGet_CanBeCalledMultipleTimes()
    {
        // Arrange
        var pageModel = new PrivacyModel();

        // Act & Assert - Should not throw on multiple calls
        pageModel.OnGet();
        pageModel.OnGet();
        pageModel.OnGet();
    }
}
