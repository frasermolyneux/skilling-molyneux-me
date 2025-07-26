using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using MX.Skilling.Web.Pages;
using Xunit;

namespace MX.Skilling.Web.Tests.Pages;

/// <summary>
/// Unit tests for ManageGraphModel.
/// </summary>
[Trait("Category", "Unit")]
public class ManageGraphModelTests
{
    private readonly Mock<ILogger<ManageGraphModel>> _mockLogger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManageGraphModelTests"/> class.
    /// </summary>
    public ManageGraphModelTests()
    {
        _mockLogger = new Mock<ILogger<ManageGraphModel>>();
    }

    /// <summary>
    /// Tests that the OnGet method logs user access.
    /// </summary>
    [Fact]
    public void OnGet_LogsUserAccess()
    {
        // Arrange
        var model = new ManageGraphModel(_mockLogger.Object);
        var identity = new ClaimsIdentity("test");
        identity.AddClaim(new Claim(ClaimTypes.Name, "Test User"));
        model.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        // Act
        model.OnGet();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ManageGraph page accessed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
    /// <inheritdoc/>

    /// <summary>
    /// Tests that the ManageGraphModel has the correct Authorize attribute.
    /// </summary>
    [Fact]
    public void ManageGraphModel_HasAuthorizeAttribute()
    {
        // Arrange & Act
        var authorizeAttribute = typeof(ManageGraphModel)
            .GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("AdminPolicy", authorizeAttribute.Policy);
    }
}
