using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MX.Skilling.Web.Pages;

namespace MX.Skilling.Web.Tests.Pages;

/// <summary>
/// Unit tests for the <see cref="ErrorModel"/> page model.
/// </summary>
public class ErrorModelTests
{
    /// <summary>
    /// Verifies that OnGetAsync method sets RequestId from HttpContext.TraceIdentifier when Activity.Current is null.
    /// </summary>
    [Fact]
    public async Task OnGetAsync_WithNoActivity_SetsRequestIdFromTraceIdentifier()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ErrorModel>>();
        var pageModel = new ErrorModel(mockLogger.Object);
        var httpContext = new DefaultHttpContext
        {
            TraceIdentifier = "test-trace-id"
        };
        var pageContext = new PageContext
        {
            HttpContext = httpContext
        };
        pageModel.PageContext = pageContext;

        // Act
        await pageModel.OnGetAsync();

        // Assert
        Assert.Equal("test-trace-id", pageModel.RequestId);
        Assert.True(pageModel.ShowRequestId);

        // Verify error logging was called
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error page accessed with RequestId")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that ShowRequestId returns false when RequestId is null or empty.
    /// </summary>
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("   ", true)] // Whitespace is considered non-empty by IsNullOrEmpty
    [InlineData("request-id", true)]
    public void ShowRequestId_WithVariousInputs_ReturnsExpectedResult(string? requestId, bool expected)
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ErrorModel>>();
        var pageModel = new ErrorModel(mockLogger.Object)
        {
            RequestId = requestId
        };

        // Act
        var result = pageModel.ShowRequestId;

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies that constructor throws ArgumentNullException when logger is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new ErrorModel(null!));
        Assert.Equal("logger", exception.ParamName);
    }
}
