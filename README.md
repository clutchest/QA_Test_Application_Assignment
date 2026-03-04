# TestAssignment - Playwright UI Tests

End-to-end UI automation tests for the registration and login flow using **Playwright for .NET** + **NUnit**.

Tests cover the registration form at: https://qa-test-web-app.vercel.app/register.html

## Project Overview

Automated tests for:
- Required field validation (all fields except newsletter)
- Format validation (email, password length, etc.)
- Negative scenarios (duplicate email, mismatch passwords, missing terms, etc.)
- Happy path (successful registration → login → dashboard)
- Navigation (Already have an account link)

## Project Structure
Solution 'TestAssignment' (1 of 1 project)
└── TestAssignment (test project)
├── Dependencies
├── Properties
│   └── launchSettings.json
├── Helpers
│   └── TestData.cs               # Random user data generator
├── Pages                         # Page Object Model
│   ├── BasePage.cs
│   ├── RegisterPage.cs
│   ├── LoginPage.cs
│   └── DashboardPage.cs
├── Tests
│   └── RegisterTests.cs          # All TC01–TC18 tests
├── README.md
└── test.runsettings              # Required


## Tech Stack

- .NET 8.0
- Microsoft.Playwright
- NUnit + Microsoft.Playwright.NUnit
- Page Object Model pattern
- Random test data generation

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022/2025 (or VS Code + .NET CLI)
- Playwright browsers (installed once)

## Setup from zero

1. Clone or download the repository
2. Open solution in Visual Studio (or run `dotnet restore` in terminal)
3. Install Playwright browsers (run once!):

```bash
# In terminal, from project root
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install --with-deps

(If pwsh not recognized: dotnet tool install --global PowerShell first)



## How to Run Tests
In Visual Studio (recommended)

Open solution
Build solution (Ctrl+Shift+B)
Open Test Explorer (Test → Test Explorer)
Click Run All (or right-click group → Run)

To run in headed mode (see browser):
Test → Configure Run Settings → Select Solution Wide runsettings File → pick the file

From command line
# All tests - headless
dotnet test

# Headed mode (see browser)
dotnet test -p:Headless=false

# Single test
dotnet test --filter Name~TC01

# With trace on failure
dotnet test -p:Trace=true



## Troubleshooting

Browsers not launching: Run playwright.ps1 install --with-deps again
Tests not discovered in VS: Rebuild solution, make sure NUnit3TestAdapter is installed
False positives in negative tests: Increase Task.Delay or use WaitForLoadStateAsync(LoadState.NetworkIdle)
Flaky tests: Add trace (-p:Trace=true) and view with playwright.ps1 show-trace

Happy testing! 🚀

Last updated: February 2026
