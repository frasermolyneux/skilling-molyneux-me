# Playwright Installation Script
# This script installs Playwright browsers needed for UI testing

Write-Host "Installing Playwright browsers..." -ForegroundColor Green

# Get the script's directory to ensure we're working from the correct location
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$projectDir = $scriptDir
$projectFile = Join-Path $projectDir "MX.Skilling.Web.UITests.csproj"

# Build the project first to ensure the playwright executable is available
Write-Host "Building UI tests project..." -ForegroundColor Yellow
dotnet build $projectFile

# Install Playwright browsers using the project-local installation
$playwrightScript = Join-Path $projectDir "bin/Debug/net9.0/playwright.ps1"
if (Test-Path $playwrightScript) {
    Write-Host "Installing browsers using: $playwrightScript" -ForegroundColor Yellow
    pwsh $playwrightScript install chromium firefox webkit
}
else {
    Write-Error "Playwright script not found at: $playwrightScript"
    Write-Error "Make sure the project builds successfully first."
    exit 1
}

Write-Host "Playwright browsers installed successfully!" -ForegroundColor Green
Write-Host "You can now run UI tests using:" -ForegroundColor Yellow
Write-Host "  dotnet test src/MX.Skilling.Web.UITests/" -ForegroundColor Yellow
