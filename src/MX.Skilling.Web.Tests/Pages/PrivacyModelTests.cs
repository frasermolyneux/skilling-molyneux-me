using Microsoft.Extensions.Logging;
using MX.Skilling.Web.Pages;

namespace MX.Skilling.Web.Tests.Pages;

/// <summary>
/// Unit tests for the <see cref="PrivacyModel"/> page model.
/// </summary>
public class PrivacyModelTests
{
    /// <summary>
    /// Verifies that OnGetAsync method completes successfully without throwing exceptions.
    /// </summary>
    [Fact]
    public async Task OnGetAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<PrivacyModel>>();
        var pageModel = new PrivacyModel(mockLogger.Object);

        // Act & Assert - Should not throw
        await pageModel.OnGetAsync();

        // Verify logging was called
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Privacy policy page accessed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that constructor throws ArgumentNullException when logger is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new PrivacyModel(null!));
        Assert.Equal("logger", exception.ParamName);
    }
}
