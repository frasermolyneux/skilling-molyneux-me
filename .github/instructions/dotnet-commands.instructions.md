---
applyTo: '**'
---

# .NET Commands for MX.Skilling Solution

This document provides the standard .NET CLI commands to work with the MX.Skilling solution. *Always* prefer direct command execution over the tasks defined in the tasks.json file, as the tasks may not be up-to-date with the latest changes in the solution structure.

## Prerequisites

Ensure you have .NET 9.0 SDK installed on your machine.

## Solution Commands

### Clean Solution
```bash
dotnet clean src/MX.Skilling.sln
```

### Restore Dependencies
```bash
dotnet restore src/MX.Skilling.sln
```

### Build Solution
```bash
dotnet build src/MX.Skilling.sln
```

### Build Solution (Release Configuration)
```bash
dotnet build src/MX.Skilling.sln --configuration Release
```

### Test Solution (All Tests)
```bash
dotnet test src/MX.Skilling.sln
```

### Test Categories

The solution includes three distinct test categories with different requirements:

#### Unit Tests (Fast, No Dependencies)
```bash
dotnet test src/MX.Skilling.sln --filter "Category=Unit"
```

#### Integration Tests (Requires Web App Context)
```bash
dotnet test src/MX.Skilling.sln --filter "Category=Integration"
```

#### Unit & Integration Tests Combined (Recommended for CI/CD)
```bash
dotnet test src/MX.Skilling.sln --filter "Category=Unit|Category=Integration"
```

#### UI Tests (Requires Running Web Application)
**⚠️ Important:** UI tests require the web application to be running first!

1. **Start the web application** (in a separate terminal):
   ```bash
   dotnet run --project src/MX.Skilling.Web
   ```

2. **Run UI tests** (in another terminal):
   ```bash
   dotnet test src/MX.Skilling.Web.UITests
   ```

   Or with filter:
   ```bash
   dotnet test src/MX.Skilling.sln --filter "Category=UI"
   ```

#### UI Tests Debug Mode
For debugging UI tests with browser visible:
```bash
# Set environment variable and run
$env:PLAYWRIGHT_DEBUG="1"
dotnet test src/MX.Skilling.Web.UITests
```

### Test with Coverage
```bash
# All tests
dotnet test src/MX.Skilling.sln --collect:"XPlat Code Coverage"

# Unit and Integration only (recommended for coverage reports)
dotnet test src/MX.Skilling.sln --filter "Category=Unit|Category=Integration" --collect:"XPlat Code Coverage"
```

### Run Web Application
```bash
dotnet run --project src/MX.Skilling.Web
```

### Watch and Run Web Application (Hot Reload)
```bash
dotnet watch --project src/MX.Skilling.Web
```

## Individual Project Commands

### Clean Individual Project
```bash
dotnet clean src/MX.Skilling.Web
dotnet clean src/MX.Skilling.Web.Tests
dotnet clean src/MX.Skilling.Web.UITests
```

### Build Individual Project
```bash
dotnet build src/MX.Skilling.Web
dotnet build src/MX.Skilling.Web.Tests
dotnet build src/MX.Skilling.Web.UITests
```

### Test Individual Project
```bash
# Unit and Integration tests
dotnet test src/MX.Skilling.Web.Tests

# UI tests (requires web app running)
dotnet test src/MX.Skilling.Web.UITests
```

## Development Workflow

### Standard Development Cycle
1. **Clean and Restore**: `dotnet clean src/MX.Skilling.sln && dotnet restore src/MX.Skilling.sln`
2. **Build**: `dotnet build src/MX.Skilling.sln`
3. **Test (Unit & Integration)**: `dotnet test src/MX.Skilling.sln --filter "Category=Unit|Category=Integration"`
4. **Run**: `dotnet run --project src/MX.Skilling.Web`

### Full Testing Workflow (Including UI Tests)
1. **Clean, Restore, Build**: `dotnet clean src/MX.Skilling.sln && dotnet restore src/MX.Skilling.sln && dotnet build src/MX.Skilling.sln`
2. **Unit & Integration Tests**: `dotnet test src/MX.Skilling.sln --filter "Category=Unit|Category=Integration"`
3. **Start Web App**: `dotnet run --project src/MX.Skilling.Web` *(in separate terminal)*
4. **UI Tests**: `dotnet test src/MX.Skilling.sln --filter "Category=UI"` *(in another terminal)*

### Quick Development Commands
```bash
# Standard validation sequence
dotnet clean src/MX.Skilling.sln && dotnet restore src/MX.Skilling.sln && dotnet build src/MX.Skilling.sln && dotnet test src/MX.Skilling.sln --filter "Category=Unit|Category=Integration" && dotnet format src/MX.Skilling.sln --verify-no-changes
```

## Useful Additional Commands

### Add Package Reference
```bash
dotnet add src/MX.Skilling.Web package [PackageName]
dotnet add src/MX.Skilling.Web.Tests package [PackageName]
dotnet add src/MX.Skilling.Web.UITests package [PackageName]
```

### List Package References
```bash
dotnet list src/MX.Skilling.Web package
dotnet list src/MX.Skilling.Web.Tests package
dotnet list src/MX.Skilling.Web.UITests package
```

### Format Code
```bash
dotnet format src/MX.Skilling.sln
```

### Verify Code Format
```bash
dotnet format src/MX.Skilling.sln --verify-no-changes
```

### Playwright Browser Installation (UI Tests Setup)
```bash
# Install Playwright browsers (required for UI tests)
pwsh -ExecutionPolicy Bypass -File src/MX.Skilling.Web.UITests/install-browsers.ps1
```
