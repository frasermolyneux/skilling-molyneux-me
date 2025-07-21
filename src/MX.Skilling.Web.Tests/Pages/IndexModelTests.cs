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
}
