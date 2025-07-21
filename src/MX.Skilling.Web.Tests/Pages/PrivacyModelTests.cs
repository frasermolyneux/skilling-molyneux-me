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
}
