---
applyTo: '**/*.cshtml'
---

# Razor Views Guidelines

## View Structure & Organization

### Layout and Partials
- **Use consistent layout structure** - inherit from `_Layout.cshtml`
- **Extract reusable components** into partial views
- **Keep views focused** - avoid complex logic

```html
@{
    ViewData["Title"] = "Users";
}

<h1>@ViewData["Title"]</h1>

<partial name="_UserList" model="Model.Users" />
```

### Model Binding
- **Use strongly-typed views** - avoid `ViewBag`/`ViewData` for complex data
- **Leverage `@model` directive** for IntelliSense and type safety
- **Use display templates** for consistent formatting

```csharp
@model UsersModel

@foreach (var user in Model.Users)
{
    @Html.DisplayFor(m => user, "UserSummary")
}
```

## HTML & Accessibility

### Semantic HTML
- **Use semantic HTML elements** (`<main>`, `<section>`, `<article>`)
- **Include proper heading hierarchy** (h1, h2, h3...)
- **Add ARIA labels** where context isn't clear

```html
<main>
    <section aria-labelledby="users-heading">
        <h2 id="users-heading">Active Users</h2>
        <ul role="list">
            @foreach (var user in Model.Users)
            {
                <li>@user.Name</li>
            }
        </ul>
    </section>
</main>
```

### Forms
- **Use Tag Helpers** for form elements
- **Include validation** with client and server-side support
- **Provide clear labels** and error messages

```html
<form method="post">
    <div class="form-group">
        <label asp-for="Request.Email"></label>
        <input asp-for="Request.Email" class="form-control" />
        <span asp-validation-for="Request.Email" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Create User</button>
</form>
```

## Performance & Best Practices

### Conditional Rendering
- **Use `@if` for conditional content** rather than CSS hiding
- **Avoid complex expressions** in views - use computed properties
- **Cache expensive operations** in the page model

```html
@if (Model.Users.Any())
{
    <div class="user-grid">
        @foreach (var user in Model.Users)
        {
            <partial name="_UserCard" model="user" />
        }
    </div>
}
else
{
    <p class="no-users">No users found.</p>
}
```

### CSS and JavaScript
- **Use CSS classes** over inline styles
- **Leverage sections** for page-specific scripts/styles
- **Minimize JavaScript in views** - prefer separate files

```html
@section Scripts {
    <script src="~/js/users.js"></script>
}

@section Styles {
    <link rel="stylesheet" href="~/css/users.css" />
}
```

## Automation & Testing Support

### Automation IDs
- **Add data-automation-id attributes** to interactive elements for UI testing
- **Use descriptive, stable identifiers** that won't change with UI updates
- **Target buttons, inputs, and key content areas** for test automation

```html
<!-- Forms and inputs -->
<form method="post">
    <div class="form-group">
        <label asp-for="Request.Email">Email Address</label>
        <input asp-for="Request.Email"
               class="form-control"
               data-automation-id="email-input" />
        <span asp-validation-for="Request.Email" class="text-danger"></span>
    </div>

    <button type="submit"
            class="btn btn-primary"
            data-automation-id="submit-button">
        Create User
    </button>
</form>

<!-- Navigation and interactive elements -->
<nav>
    <a href="/users" data-automation-id="users-nav-link">Users</a>
    <a href="/dashboard" data-automation-id="dashboard-nav-link">Dashboard</a>
</nav>

<!-- Content areas for verification -->
<div class="user-profile" data-automation-id="user-profile-section">
    <h2 data-automation-id="user-name">@Model.User.Name</h2>
    <p data-automation-id="user-email">@Model.User.Email</p>
</div>

<!-- Status and feedback messages -->
@if (Model.IsSuccess)
{
    <div class="alert alert-success" data-automation-id="success-message">
        User created successfully!
    </div>
}
```

### Automation ID Guidelines
- **Use kebab-case naming** (e.g., `user-details-section`)
- **Include element type** where helpful (`-button`, `-input`, `-link`)
- **Be specific but not brittle** - avoid implementation details
- **Group related elements** with common prefixes (`user-profile-name`, `user-profile-email`)

## Error Handling
- **Display user-friendly error messages**
- **Show validation summary** when model state is invalid
- **Provide fallback content** for missing data

```html
@if (!ViewData.ModelState.IsValid)
{
    <div asp-validation-summary="All" class="alert alert-danger"></div>
}
```
