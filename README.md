# AppsChangeLog

An ASP.NET Core MVC project for listing company applications and their vendors, current versions, changelogs, and import instructions.

## Current status

Initial MVC scaffold with placeholder application, vendor, and user models. Database integration and account management are planned.

## Technology

- ASP.NET Core MVC
- .NET 8
- Razor views and Bootstrap

## Run locally

Install the .NET 8 SDK, then run from the repository root:

```powershell
dotnet restore AppsChangeLog.sln
dotnet run --project AppsChangeLog/AppsChangeLog.csproj --launch-profile http
```

Open the local URL printed in the terminal. No database or credentials are required for the current scaffold.
