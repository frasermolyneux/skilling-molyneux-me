---
applyTo: '**/*.cs'
---

# Testing Strategy

## Core Testing Principles

**Comprehensive testing strategy** using established frameworks and best practices:

- **Unit Tests** - Fast, isolated tests using xUnit and Moq
- **Integration Tests** - Component interaction testing
- **UI Tests** - End-to-end browser testing using Playwright
- **Test-Driven Development** - Write tests before implementation when possible

## Testing Framework Stack

### Unit & Integration Testing
- **xUnit** - Primary testing framework for all .NET tests
- **Moq** - Mocking and stubbing for unit tests
- **coverlet.collector** - Code coverage collection
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing for ASP.NET Core

### UI Testing
- **Microsoft.Playwright** - Cross-browser UI automation
- **Microsoft.Playwright.Xunit** - Playwright integration with xUnit for UI tests

## Unit Testing Guidelines

### Test Structure & Naming
```csharp
// Follow Arrange-Act-Assert pattern
[Fact]
public void ProcessUser_WithValidInput_ReturnsExpectedResult()
{
    // Arrange
    var mockRepository = new Mock<IUserRepository>();
    var service = new UserService(mockRepository.Object);
    var validUser = new User { Id = 1, Name = "John Doe" };

    // Act
    var result = service.ProcessUser(validUser);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("John Doe", result.Name);
}
```

### Test Categories
```csharp
// Use Theory for data-driven tests
[Theory]
[InlineData("", false)]
[InlineData(null, false)]
[InlineData("valid@email.com", true)]
public void ValidateEmail_WithVariousInputs_ReturnsExpectedResult(string email, bool expected)
{
    var result = EmailValidator.IsValid(email);
    Assert.Equal(expected, result);
}

// Use Fact for single scenario tests
[Fact]
public void CreateUser_WithDuplicateEmail_ThrowsArgumentException()
{
    var exception = Assert.Throws<ArgumentException>(() => service.CreateUser(duplicateUser));
    Assert.Contains("email already exists", exception.Message);
}
```

### Mocking Best Practices
```csharp
// Mock dependencies, not data
var mockLogger = new Mock<ILogger<UserService>>();
var mockRepository = new Mock<IUserRepository>();

// Setup specific behaviors
mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
             .ReturnsAsync(new User { Id = 1, Name = "Test User" });

// Verify interactions when important
mockRepository.Verify(r => r.SaveAsync(It.IsAny<User>()), Times.Once);
```

## Integration Testing Guidelines

### ASP.NET Core Integration Tests
```csharp
public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UserControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUser_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/users/1");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString());
    }
}
```

## UI Testing with Playwright

### Test Setup
```csharp
public class UserInterfaceTests : PageTest
{
    [Fact]
    public async Task LoginForm_WithValidCredentials_NavigatesToDashboard()
    {
        // Arrange
        await Page.GotoAsync("https://localhost:5001");
        await Page.FillAsync("[data-automation-id='email-input']", "test@example.com");
        await Page.FillAsync("[data-automation-id='password-input']", "ValidPassword123!");

        // Act
        await Page.ClickAsync("[data-automation-id='login-button']");

        // Assert
        await Expect(Page).ToHaveURLAsync(new Regex(".*/dashboard"));
        await Expect(Page.Locator("h1")).ToContainTextAsync("Dashboard");
    }
}
```

### UI Testing Best Practices
```csharp
// Use data-automation-id attributes for stable selectors
await Page.ClickAsync("[data-automation-id='submit-button']");

// Wait for elements and state changes
await Page.WaitForSelectorAsync("[data-automation-id='success-message']");
await Expect(Page.Locator("[data-automation-id='user-name']")).ToBeVisibleAsync();

// Test critical user paths
[Fact]
public async Task UserRegistration_CompleteFlow_CreatesAccountSuccessfully()
{
    // Arrange
    await Page.GotoAsync("https://localhost:5001/register");

    // Fill registration form
    await Page.FillAsync("[data-automation-id='first-name-input']", "John");
    await Page.FillAsync("[data-automation-id='last-name-input']", "Doe");
    await Page.FillAsync("[data-automation-id='email-input']", "john@example.com");

    // Act
    await Page.ClickAsync("[data-automation-id='register-button']");

    // Assert
    await Expect(Page.Locator("[data-automation-id='welcome-message']"))
        .ToContainTextAsync("Welcome, John!");
}
```

## Test Organization

### Project Structure
```
src/
├── MX.Skilling.Web/
├── MX.Skilling.Web.Tests/          # Unit & Integration tests
│   ├── Controllers/
│   ├── Services/
│   ├── Models/
│   └── Integration/
└── MX.Skilling.Web.UITests/        # Playwright UI tests
    ├── Pages/
    ├── Tests/
    └── Support/
```

### Test Categories and Traits
```csharp
// Unit tests
[Trait("Category", "Unit")]
public class UserServiceTests { }

// Integration tests
[Trait("Category", "Integration")]
public class UserControllerIntegrationTests { }

// UI tests
[Trait("Category", "UI")]
public class LoginPageTests { }
```

## Coverage and Quality Gates

### Code Coverage Targets
- **Unit Tests**: Minimum 80% code coverage
- **Critical Paths**: 100% coverage for business logic
- **UI Tests**: Cover all major user workflows

### Test Execution Strategy
```bash
# Run all tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test categories
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"

# Run UI tests (separate project)
dotnet test src/MX.Skilling.Web.UITests/
```

## Continuous Integration

### Test Pipeline Requirements
1. **Unit Tests** - Run on every commit
2. **Integration Tests** - Run on pull requests
3. **UI Tests** - Run on main branch and releases
4. **Coverage Reports** - Generate and enforce thresholds

### Performance Guidelines
- **Unit Tests**: Each test should complete in < 100ms
- **Integration Tests**: Full suite should complete in < 5 minutes
- **UI Tests**: Critical path tests should complete in < 10 minutes

## Best Practices Summary

### Do's ✅
- **Write tests first** when implementing new features
- **Test behavior, not implementation** details
- **Use descriptive test names** that explain the scenario
- **Mock external dependencies** in unit tests
- **Test edge cases** and error conditions
- **Keep tests simple and focused** on one concern
- **Use page object pattern** for UI tests
- **Use automation IDs** for stable UI test selectors

### Don'ts ❌
- **Don't test framework code** (ASP.NET Core, Entity Framework)
- **Don't write brittle tests** that break with UI changes
- **Don't ignore failing tests** - fix or remove them
- **Don't test private methods** directly
- **Don't create test dependencies** on external systems in unit tests
- **Don't duplicate logic** between test and production code

### Test Data Management
```csharp
// Use builders for complex test data
public class UserBuilder
{
    private User _user = new User();

    public UserBuilder WithName(string name)
    {
        _user.Name = name;
        return this;
    }

    public User Build() => _user;
}

// Usage in tests
var testUser = new UserBuilder()
    .WithName("John Doe")
    .WithEmail("john@example.com")
    .Build();
```
