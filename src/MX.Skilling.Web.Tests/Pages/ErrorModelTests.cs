using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MX.Skilling.Web.Pages;

namespace MX.Skilling.Web.Tests.Pages;

/// <summary>
/// Unit tests for the <see cref="ErrorModel"/> page model.
/// </summary>
[Trait("Category", "Unit")]
public class ErrorModelTests
{
    /// <summary>
    /// Verifies that OnGet method sets RequestId from HttpContext.TraceIdentifier when Activity.Current is null.
    /// </summary>
    [Fact]
    public void OnGet_WithNoActivity_SetsRequestIdFromTraceIdentifier()
    {
        // Arrange
        var pageModel = new ErrorModel();
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
        pageModel.OnGet();

        // Assert
        Assert.Equal("test-trace-id", pageModel.RequestId);
        Assert.True(pageModel.ShowRequestId);
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
        var pageModel = new ErrorModel
        {
            RequestId = requestId
        };

        // Act
        var result = pageModel.ShowRequestId;

        // Assert
        Assert.Equal(expected, result);
    }
}
