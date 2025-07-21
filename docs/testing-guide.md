# Testing Guide

This document provides guidance on running tests in the MX.Skilling project. The testing strategy includes unit tests, integration tests, and UI tests using industry-standard tools and frameworks.

## Test Categories

### 1. Unit Tests
- **Location**: `src/MX.Skilling.Web.Tests/Pages/`
- **Framework**: xUnit with Moq for mocking
- **Purpose**: Test individual components in isolation
- **Coverage**: Page models, services, and business logic

### 2. Integration Tests
- **Location**: `src/MX.Skilling.Web.Tests/Integration/`
- **Framework**: xUnit with ASP.NET Core Test Host
- **Purpose**: Test component interactions and HTTP endpoints
- **Coverage**: End-to-end request/response flows

### 3. UI Tests
- **Location**: `src/MX.Skilling.Web.UITests/`
- **Framework**: xUnit with Microsoft Playwright
- **Purpose**: Test user interface and browser interactions
- **Coverage**: Cross-browser end-to-end scenarios

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- For UI tests: Playwright browsers (installed automatically)

### Running All Tests
```bash
# Build and run all tests
dotnet test src/MX.Skilling.sln
```

### Running Specific Test Categories
```bash
# Unit tests only
dotnet test src/MX.Skilling.sln --filter "Category=Unit"

# Integration tests only
dotnet test src/MX.Skilling.sln --filter "Category=Integration"

# UI tests only
dotnet test src/MX.Skilling.Web.UITests/
```

## Visual Studio Code Tasks

The project includes pre-configured VS Code tasks for easy testing:

### Available Tasks
- **`test`** - Run all tests (default)
- **`test-unit`** - Run unit tests only
- **`test-integration`** - Run integration tests only
- **`test-ui`** - Run UI tests
- **`test-ui-debug`** - Run UI tests with visible browser
- **`test-with-coverage`** - Run tests with code coverage
- **`install-playwright-browsers`** - Install Playwright browsers

### Using Tasks
1. Open Command Palette (`Ctrl+Shift+P`)
2. Type "Tasks: Run Task"
3. Select the desired test task

## UI Testing Setup

### First-Time Setup
```bash
# Install Playwright browsers (one-time setup)
dotnet run --project src/MX.Skilling.Web.UITests/ -- install
```

Or use the VS Code task: `install-playwright-browsers`

### Running UI Tests

#### Headless Mode (Default)
```bash
dotnet test src/MX.Skilling.Web.UITests/
```

#### Debug Mode (Visible Browser)
```bash
# Set environment variable and run
$env:PLAYWRIGHT_DEBUG="1"
dotnet test src/MX.Skilling.Web.UITests/
```

Or use the VS Code task: `test-ui-debug`

### UI Test Configuration

#### Application URL Configuration
The UI tests automatically detect the application URL from environment variables:

- **WEB_APP_URL**: Explicit URL for tests (highest priority)
- **ASPNETCORE_URLS**: URLs from the running web application
- **Default**: `https://localhost:7053` (matches launchSettings.json)

```bash
# Local development (automatic detection)
dotnet test src/MX.Skilling.Web.UITests/

# Override URL for testing
$env:WEB_APP_URL="https://localhost:5001"
dotnet test src/MX.Skilling.Web.UITests/

# Pipeline/CI scenario with custom URL
$env:WEB_APP_URL="https://my-app.azurewebsites.net"
dotnet test src/MX.Skilling.Web.UITests/
```

#### Other Configuration
- **Default Browser**: Chromium
- **Supported Browsers**: Chromium, Firefox, WebKit
- **Test Elements**: Uses `data-automation-id` attributes for stable selectors

## Coverage Reporting

### Generate Coverage Report
```bash
dotnet test src/MX.Skilling.sln --collect:"XPlat Code Coverage"
```

Coverage reports are generated in `TestResults/` directories within each test project.

## Integrated Development Workflow

### Web App + UI Testing Workflow

For UI test development, you'll want to run the web application and tests together:

#### Option 1: VS Code Launch Compound
1. Open the Run and Debug panel (`Ctrl+Shift+D`)
2. Select "Launch Web App for UI Testing" from the dropdown
3. Click Start (F5)
4. The web app starts in watch mode and opens in your browser
5. In another terminal, run UI tests: `dotnet test src/MX.Skilling.Web.UITests/`

#### Option 2: Manual Terminal Workflow
```bash
# Terminal 1: Start web app in watch mode
dotnet watch --project src/MX.Skilling.Web run

# Terminal 2: Run UI tests (automatically detects the running app)
dotnet test src/MX.Skilling.Web.UITests/
```

#### How URL Detection Works
1. **Automatic Detection**: UI tests automatically detect the web app URL from `ASPNETCORE_URLS`
2. **Override for Testing**: Set `WEB_APP_URL` environment variable to test against different URLs
3. **CI/CD Ready**: Same tests work locally and in pipelines with different URLs

```bash
# Local development (automatic)
dotnet test src/MX.Skilling.Web.UITests/

# Testing against deployed app
$env:WEB_APP_URL="https://my-staging-app.azurewebsites.net"
dotnet test src/MX.Skilling.Web.UITests/
```

## Test Organization

### File Structure
```
src/
├── MX.Skilling.Web.Tests/          # Unit & Integration Tests
│   ├── Pages/                      # Page model unit tests
│   └── Integration/                # Integration tests
└── MX.Skilling.Web.UITests/        # UI Tests
    ├── Tests/                      # Test classes
    ├── Pages/                      # Page object models
    └── Support/                    # Base classes and utilities
```

### Naming Conventions
- **Unit Tests**: `{ClassName}Tests.cs`
- **Integration Tests**: `{FeatureArea}IntegrationTests.cs`
- **UI Tests**: `{PageName}Tests.cs`

## Writing Tests

### Unit Test Example
```csharp
[Trait("Category", "Unit")]
public class UserServiceTests
{
    [Fact]
    public void ProcessUser_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var service = new UserService();
        var user = new User { Name = "Test" };

        // Act
        var result = service.ProcessUser(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
    }
}
```

### Integration Test Example
```csharp
[Trait("Category", "Integration")]
public class WebApplicationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetHomePage_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
```

### UI Test Example
```csharp
[Trait("Category", "UI")]
public class HomePageTests : PlaywrightTestBase
{
    [Fact]
    public async Task HomePage_DisplaysWelcomeMessage()
    {
        // Arrange
        var homePage = new HomePage(Page!);
        
        // Act
        await NavigateToAsync();
        
        // Assert
        await Expect(homePage.WelcomeHeading).ToBeVisibleAsync();
    }
}
```

## Troubleshooting

### Common Issues

#### UI Tests Not Running
1. Ensure Playwright browsers are installed:
   ```bash
   playwright install
   ```

2. Check that the web application is accessible at the configured URL

#### Integration Tests Failing
1. Verify the application builds successfully
2. Check for port conflicts
3. Ensure all dependencies are restored

#### Coverage Reports Not Generated
1. Ensure coverlet.collector package is installed
2. Use the correct coverage collection argument:
   ```bash
   --collect:"XPlat Code Coverage"
   ```

### Getting Help
1. Check test output for specific error messages
2. Use debug mode for UI tests to see browser interactions
3. Review test logs in the VS Code Terminal
4. Consult the [Testing Strategy Documentation](.github/instructions/testing-strategy.instructions.md)

## Best Practices

### Do's ✅
- Use descriptive test names that explain the scenario
- Follow Arrange-Act-Assert pattern
- Use `data-automation-id` attributes for UI element selection
- Test both happy path and edge cases
- Keep tests independent and isolated

### Don'ts ❌
- Don't test framework code (ASP.NET Core internals)
- Don't write tests that depend on external systems
- Don't use brittle selectors in UI tests
- Don't ignore failing tests
- Don't test implementation details, test behavior

## Performance Guidelines
- **Unit Tests**: < 100ms per test
- **Integration Tests**: < 5 minutes total suite
- **UI Tests**: < 10 minutes for critical path tests
