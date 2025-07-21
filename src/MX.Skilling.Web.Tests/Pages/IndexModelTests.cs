using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MX.Skilling.Web.Pages;

namespace MX.Skilling.Web.Tests.Pages;

/// <summary>
/// Unit tests for the <see cref="IndexModel"/> page model.
/// </summary>
[Trait("Category", "Unit")]
public class IndexModelTests
{
    /// <summary>
    /// Verifies that OnGet method completes successfully without throwing exceptions.
    /// </summary>
    [Fact]
    public void OnGet_ShouldCompleteSuccessfully()
    {
        // Arrange
        var pageModel = new IndexModel();

        // Act & Assert - Should not throw
        pageModel.OnGet();
    }

    /// <summary>
    /// Verifies that IndexModel inherits from PageModel.
    /// </summary>
    [Fact]
    public void IndexModel_ShouldInheritFromPageModel()
    {
        // Arrange & Act
        var pageModel = new IndexModel();

        // Assert
        Assert.IsAssignableFrom<PageModel>(pageModel);
    }

    /// <summary>
    /// Verifies that IndexModel can be instantiated without dependencies.
    /// </summary>
    [Fact]
    public void IndexModel_CanBeInstantiatedWithoutDependencies()
    {
        // Act & Assert - Should not throw
        var pageModel = new IndexModel();
        Assert.NotNull(pageModel);
    }

    /// <summary>
    /// Verifies that OnGet method can be called multiple times safely.
    /// </summary>
    [Fact]
    public void OnGet_CanBeCalledMultipleTimes()
    {
        // Arrange
        var pageModel = new IndexModel();

        // Act & Assert - Should not throw on multiple calls
        pageModel.OnGet();
        pageModel.OnGet();
        pageModel.OnGet();
    }
}
