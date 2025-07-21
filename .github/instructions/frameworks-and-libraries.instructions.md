---
applyTo: '**'
---

# Frameworks and Libraries Instructions

**Always use these established frameworks and libraries when implementing features or making technical decisions.**

## Core Framework Stack

- **.NET 9.0** - Target framework for all projects
- **ASP.NET Core** - Web framework (Razor Pages)

## Framework Categories

### Testing Frameworks
- **xUnit** - Primary testing framework
- **Moq** - Mocking library for unit tests
- **coverlet.collector** - Code coverage collection

### Code Quality
- **Microsoft.CodeAnalysis.Analyzers** - Static code analysis
- **EditorConfig** - Code style enforcement
- **Microsoft.SourceLink.GitHub** - Source debugging integration

## Decision Guidelines

**When implementing new features:**
1. **Use xUnit** for all unit tests
2. **Use Moq** for mocking dependencies in tests
3. **Follow .editorconfig** style rules
4. **Enable nullable reference types** for new code
5. **Add XML documentation** for public APIs

**When adding new libraries:**
- First check if existing frameworks can solve the problem
- Ensure compatibility with .NET 9.0
- Add to Directory.Build.props if used across multiple projects
- Update this instructions file with the new addition
