# Playwright Configuration

This directory contains UI tests using Playwright for cross-browser testing.

## Setup

1. **Install Playwright browsers** (one-time setup):
   ```powershell
   .\install-browsers.ps1
   ```

2. **Run the web application** in one terminal:
   ```bash
   dotnet run --project ../MX.Skilling.Web
   ```

3. **Run UI tests** in another terminal:
   ```bash
   dotnet test
   ```

## Test Configuration

- **Base URL**: https://localhost:7000 (configurable in PlaywrightTestBase)
- **Default Browser**: Chromium (configurable per test)
- **Headless Mode**: Enabled by default, disabled when debugging

## Environment Variables

- `PLAYWRIGHT_DEBUG=1`: Run tests in headed mode with slow motion
- Set via environment variables or in test configuration

## Test Structure

- `Tests/`: Contains test classes organized by feature
- `Pages/`: Page object models for maintainable UI tests
- `Support/`: Base classes and utilities for testing

## Running Specific Tests

```bash
# Run all UI tests
dotnet test --filter "Category=UI"

# Run specific test class
dotnet test --filter "ClassName=HomePageTests"

# Run tests in debug mode (headed browser)
$env:PLAYWRIGHT_DEBUG="1"; dotnet test
```
