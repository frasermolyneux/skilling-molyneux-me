---
applyTo: '**'
---

# .NET Commands for MX.Skilling Solution

This document provides the standard .NET CLI commands to work with the MX.Skilling solution.

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

### Test Solution
```bash
dotnet test src/MX.Skilling.sln
```

### Test with Coverage
```bash
dotnet test src/MX.Skilling.sln --collect:"XPlat Code Coverage"
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
```

### Build Individual Project
```bash
dotnet build src/MX.Skilling.Web
dotnet build src/MX.Skilling.Web.Tests
```

### Test Individual Project
```bash
dotnet test src/MX.Skilling.Web.Tests
```

## Development Workflow

1. **Clean and Restore**: `dotnet clean src/MX.Skilling.sln && dotnet restore src/MX.Skilling.sln`
2. **Build**: `dotnet build src/MX.Skilling.sln`
3. **Test**: `dotnet test src/MX.Skilling.sln`
4. **Run**: `dotnet run --project src/MX.Skilling.Web`

## Useful Additional Commands

### Add Package Reference
```bash
dotnet add src/MX.Skilling.Web package [PackageName]
dotnet add src/MX.Skilling.Web.Tests package [PackageName]
```

### List Package References
```bash
dotnet list src/MX.Skilling.Web package
dotnet list src/MX.Skilling.Web.Tests package
```

### Format Code
```bash
dotnet format src/MX.Skilling.sln
```

### Verify Code Format
```bash
dotnet format src/MX.Skilling.sln --verify-no-changes
```
