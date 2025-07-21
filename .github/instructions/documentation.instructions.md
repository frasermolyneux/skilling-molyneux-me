---
applyTo: '**'
---

# Documentation Standards

## Core Principles

**Only add documentation if it provides genuine value.** Documentation should:

- **Represent current state** - Always reflect the current version of the code, not historical changes or implementation stories
- **Be concise and actionable** - Focus on what developers need to know, not how we got there
- **Include practical examples** - Provide code samples when they clarify usage or implementation

## Documentation Structure

### Repository README
- **High-level and generic** - Describe what the solution does, not implementation details
- **Target audience**: New developers, stakeholders, and external contributors
- **Contents**: Project overview, getting started, basic usage

### Project Documentation
- **Location**: `/docs` folder in markdown format
- **Scope**: Detailed technical documentation, API references, architecture decisions
- **Target audience**: Team members and maintainers

## Documentation Guidelines

### When to Document
✅ **Do document:**
- Public APIs and their usage patterns
- Complex business logic or algorithms
- Non-obvious configuration requirements
- Architecture decisions with lasting impact

❌ **Don't document:**
- Self-explanatory code
- Temporary implementation details
- Step-by-step change histories
- Obvious functionality
- **Simple page views or basic CRUD operations**
- **Logging statements for routine operations**

### Code Comments

#### Inline Comments
Use inline comments **sparingly** and only when explaining **why**, not **what**:

```csharp
// ❌ Bad: Explains what the code does (obvious)
var users = await repository.GetUsersAsync();

// ❌ Bad: Logging obvious operations
_logger.LogInformation("Home page accessed"); // User can see this in access logs

// ✅ Good: Explains why this approach is used
// Using parallel execution here to avoid timeout on large datasets
var tasks = batches.Select(batch => ProcessBatchAsync(batch));
await Task.WhenAll(tasks);

// ✅ Good: Explains business reasoning
// Cache for 5 minutes to balance performance with data freshness
cache.Set(key, result, TimeSpan.FromMinutes(5));

// ✅ Good: Logging meaningful business events
_logger.LogWarning("Payment processing failed for order {OrderId}", orderId);
```

### Logging Guidelines

#### When to Add Logging
✅ **Do log:**
- **Business-critical events** (payments, user registration, data corruption)
- **Errors and exceptions** with context
- **Performance bottlenecks** during investigation
- **Security events** (failed login attempts, permission escalations)

❌ **Don't log:**
- **Simple page views** - this is captured by web server logs
- **Basic CRUD operations** without business significance
- **Constructor calls or method entry** unless debugging specific issues
- **Successful completion of trivial operations**

#### XML Documentation
For public APIs, include **concise** XML comments with:
- **Summary**: Brief description of purpose
- **Parameters**: What each parameter represents
- **Returns**: What the method returns
- **Exceptions**: What exceptions can be thrown and when

```csharp
/// <summary>
/// Validates user permissions for the specified resource.
/// </summary>
/// <param name="userId">The user identifier to validate.</param>
/// <param name="permission">The permission string to check (e.g., "resource:read").</param>
/// <returns>A validation result indicating success or failure.</returns>
/// <exception cref="ArgumentNullException">Thrown when userId or permission is null.</exception>
/// <exception cref="UnauthorizedAccessException">Thrown when user lacks system access.</exception>
public async Task<ValidationResult> ValidateAsync(string userId, string permission)
```

### Code Examples
Always include practical examples for:
- API usage patterns
- Configuration examples
- Common implementation scenarios

### Maintenance
- **Update with code changes** - Documentation updates are part of the feature implementation
- **Remove outdated content** - Delete documentation that no longer applies
- **Validate accuracy** - Ensure examples compile and work as documented
