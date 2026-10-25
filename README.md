# AppsChangeLog

AppsChangeLog keeps company applications and their current release information in one place. Each application belongs to a vendor and has a version number, release date, description of the changes, and instructions for importing the release into the company's system.

The application stores only the current release. Updating an application replaces its previous release information; it does not keep a release history.

## What you can do

After logging in, users see a dashboard with the applications they can access. Opening an application shows what changed and the steps needed to import it.

Administrators can:

- View all applications.
- Add, edit, and delete applications and vendors.
- Create vendor accounts and assign each account to a vendor.
- Delete vendor accounts. Administrator accounts and the currently signed-in account are protected from deletion.

Vendor users can:

- View applications belonging to their vendor.
- Edit those applications, including the current version, release changes, and import instructions.

Vendor users cannot access another vendor's applications or move an application to another vendor. Application creation and deletion remain administrator-only. A vendor cannot be deleted while it still has applications or user accounts.

## Technologies

- C# and .NET 8
- ASP.NET Core MVC and Razor views
- Entity Framework Core 8 and SQL Server
- ASP.NET Core Identity for accounts and roles
- Bootstrap for the interface

## Run locally

The default setup is for Windows with the .NET 8 SDK and SQL Server Express LocalDB installed. You can also use Visual Studio 2022 with the ASP.NET and web development workload and the SQL Server Express LocalDB component.

Download or clone the repository, then open a terminal in the folder containing `AppsChangeLog.sln`.

### 1. Restore dependencies

```powershell
dotnet restore AppsChangeLog.sln
```

### 2. Create the database

The default connection in `AppsChangeLog/appsettings.json` uses Windows Authentication and creates an `AppsChangeLog` database on `(localdb)\MSSQLLocalDB`. No SQL password is needed for this setup.

If you use Visual Studio, open **Tools > NuGet Package Manager > Package Manager Console**, select **AppsChangeLog** as the default project, and run:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
Update-Database
```

This applies the migrations already included in the repository. You do not need to create another migration for the initial setup.

Alternatively, use the command line. If the EF Core command-line tool is not already installed, install it once:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.21
```

Then apply the migrations:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef database update --project AppsChangeLog/AppsChangeLog.csproj
```

### 3. Start the application

```powershell
dotnet run --project AppsChangeLog/AppsChangeLog.csproj --launch-profile http
```

Open http://localhost:5291 in your browser. Keep the terminal running while using the application. Press Ctrl+C to stop it.

In Visual Studio, you can instead open the solution, select the **http** launch profile, and start the project.

### 4. Log in

On startup in Development mode, the application creates the Admin and Vendor roles and a local administrator account if they do not already exist.

| Field | Value |
| --- | --- |
| Username | `root` |
| Password | `root` |
| Alternative login email | `root@example.com` |

These credentials are for local testing. Do not use this account for a publicly deployed application. Startup does not reset the password of an existing root account.

A fresh database has no vendors or applications. Log in as root, add a vendor, and then add an application for that vendor. To test vendor access, create a user account from Manage users and assign it to that vendor. New vendor accounts use their email address as their username and must meet Identity's normal password rules.

## Use a different SQL Server

To use another SQL Server instance locally, right-click the project in Visual Studio and choose **Manage User Secrets**. Add a connection string using your server name:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER\\YOUR_INSTANCE;Database=AppsChangeLog;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
  }
}
```

User Secrets override the default connection in Development mode and stay outside the repository. Apply migrations to the selected database before starting the application. Switching the connection does not transfer records from the old database.

The included launch profiles set `ASPNETCORE_ENVIRONMENT` to `Development`. No other environment variables, API keys, or external service accounts are required for the default local setup.

## Troubleshooting

- **Unable to connect to web server https:** try the http launch profile and check the terminal output for the underlying error.
- **The executable is locked during a build:** stop debugging and any running `dotnet run` process, then rebuild.
- **Database connection fails:** check that LocalDB or your selected SQL Server instance is installed and available. Also check for a connection override in User Secrets.
- **Tables are missing:** apply the included migrations to the same database the application uses.
