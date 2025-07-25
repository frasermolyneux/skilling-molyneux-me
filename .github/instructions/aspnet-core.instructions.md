---
applyTo: '**/Pages/**/*.cs, **/Controllers/**/*.cs, **/Program.cs, **/Startup.cs'
---

# ASP.NET Core Guidelines

## Dependency Injection & Configuration

### Service Registration
- **Use extension methods** for service registration groups
- **Register services by lifetime** appropriately (Singleton, Scoped, Transient)
- **Use `IOptions<T>` pattern** for configuration

```csharp
// ✅ Good - extension method for related services
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
```

### Configuration
- **Bind configuration to strongly-typed options**
- **Validate configuration at startup**
- **Use `IOptionsSnapshot<T>` for runtime config changes**

```csharp
public class DatabaseOptions
{
    public required string ConnectionString { get; init; }
    public int CommandTimeout { get; init; } = 30;
}

// In Program.cs
builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection("Database"));
```

## Razor Pages Best Practices

### Page Models
- **Keep page models focused** - one responsibility per page
- **Use async methods** for data operations that are genuinely asynchronous
- **Don't add artificial async** - avoid `await Task.CompletedTask` for simple operations
- **Return appropriate results** (`Page()`, `RedirectToPage()`, etc.)

```csharp
// ✅ Good - genuine async operation
public class UsersModel : PageModel
{
    private readonly IUserService _userService;

    public UsersModel(IUserService userService)
    {
        _userService = userService;
    }

    public IList<User> Users { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Users = await _userService.GetActiveUsersAsync(); // Real async work
    }
}

// ✅ Good - simple synchronous operation
public class HomeModel : PageModel
{
    public void OnGet()
    {
        // Simple page view - no need for async
    }
}

// ❌ Bad - artificial async with no real async work
public class HomeModel : PageModel
{
    public async Task OnGetAsync()
    {
        await Task.CompletedTask; // This adds no value
    }
}
```

### Model Binding & Validation
- **Use data annotations** for basic validation
- **Implement `IValidatableObject`** for complex validation
- **Check `ModelState.IsValid`** before processing

```csharp
[BindProperty]
public CreateUserRequest Request { get; set; } = default!;

public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
    {
        return Page();
    }

    await _userService.CreateAsync(Request);
    return RedirectToPage("./Index");
}
```

## HTTP & Routing

### Error Handling
- **Use exception handling middleware** for global errors
- **Return appropriate HTTP status codes**
- **Handle validation errors consistently**

### Security
- **Enable CSRF protection** (default in Razor Pages)
- **Validate all inputs** at the boundary
- **Use HTTPS redirection** in production

## Logging
- **Use structured logging** with proper log levels
- **Include correlation identifiers** for tracing
- **Log at appropriate boundaries** (not in every method)
- **Focus on business value** - avoid logging obvious operations

```csharp
// ✅ Good - logs meaningful business event
_logger.LogInformation("Creating user with email {Email}", request.Email);

// ✅ Good - logs error with context
_logger.LogError("Failed to process payment for order {OrderId}: {Error}", orderId, ex.Message);

// ❌ Bad - logs obvious page access
_logger.LogInformation("Home page accessed"); // This is already in web server logs

// ❌ Bad - logs trivial operations
_logger.LogInformation("Entering OnGet method"); // Adds no business value
```
