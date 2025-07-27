using MX.Skilling.Web.Extensions;

// Application entry point and configuration
var builder = WebApplication.CreateBuilder(args);

// Add Key Vault configuration
builder.AddKeyVaultConfiguration();

// Add Application Insights telemetry
builder.Services.AddApplicationInsightsTelemetry();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHealthChecks();

// Add Azure AD authentication and authorization
builder.Services.AddAzureAdAuthentication(builder.Configuration);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// Authentication must come before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

/// <summary>
/// Program class for integration testing.
/// </summary>
public partial class Program { }
