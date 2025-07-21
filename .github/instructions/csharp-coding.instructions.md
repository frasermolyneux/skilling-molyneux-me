---
applyTo: '**/*.cs'
---

# C# Coding Guidelines

## Source of Truth
**Follow .editorconfig settings** - All formatting, naming, and style rules are enforced through .editorconfig. This document covers best practices beyond formatting.

## Core Principles

### Nullable Reference Types
- **Enable nullable reference types** for all new code
- **Use explicit nullability annotations** (`?`, `!`) when intent is clear
- **Validate inputs** at public API boundaries

```csharp
public string ProcessName(string? input)
{
    ArgumentException.ThrowIfNullOrEmpty(input);
    return input.Trim();
}
```

### Async/Await Best Practices
- **Use async/await consistently** - Don't mix with `.Result` or `.Wait()`
- **Add ConfigureAwait(false)** in library code (not required in ASP.NET Core)
- **Use Task over async void** except for event handlers
- **Suffix async methods with 'Async'**

```csharp
// ✅ Good
public async Task<User> GetUserAsync(int id)
{
    return await repository.FindAsync(id);
}

// ❌ Bad - mixing patterns
public User GetUser(int id)
{
    return repository.FindAsync(id).Result; // Can cause deadlocks
}
```

### Exception Handling
- **Use specific exception types** over generic `Exception`
- **Validate arguments** using `ArgumentException.ThrowIf*` methods
- **Don't catch and rethrow** without adding value

```csharp
public void ProcessData(IEnumerable<string> items)
{
    ArgumentNullException.ThrowIfNull(items);

    try
    {
        // Process items
    }
    catch (InvalidOperationException ex)
    {
        // Log and handle specific case
        throw new DataProcessingException("Failed to process items", ex);
    }
}
```

### LINQ and Collections
- **Use appropriate collection types** (`IReadOnlyList<T>`, `IEnumerable<T>`)
- **Prefer LINQ for readability** but be mindful of performance
- **Use collection expressions** for initialization (C# 12+)

```csharp
// ✅ Good - collection expression
private readonly string[] _validStates = ["Active", "Inactive", "Pending"];

// ✅ Good - appropriate return type
public IReadOnlyList<User> GetActiveUsers() => users.Where(u => u.IsActive).ToList();
```

### Dependency Injection
- **Use constructor injection** for required dependencies
- **Avoid service locator pattern** - inject what you need
- **Keep constructors simple** - no complex logic

```csharp
public class UserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
```

## Performance Considerations
- **Use `StringBuilder` for multiple string concatenations**
- **Consider `Span<T>` and `Memory<T>` for performance-critical code**
- **Avoid unnecessary allocations** in hot paths
- **Use `ValueTask<T>` when results are often synchronous**
