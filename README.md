# AppsChangeLog

AppsChangeLog is a web application for a company and its software vendors. It shows the current version of each application, what has changed, and how to import the new version into the company system.

Only the current version is saved. When someone updates an application, the old release information is replaced.

## Users and access

Users must log in before they can use the website.

An admin can:

- See all applications.
- Add, edit, and delete applications.
- Add, edit, and delete vendors.
- Create user accounts and choose their vendor.
- Delete vendor user accounts.

A vendor user can see and edit only the applications of their own vendor. They cannot see applications from other vendors. They cannot add or delete applications or change their vendor.

A vendor cannot be deleted if it has applications or users. Admin accounts cannot be deleted from the user management page.

## Technologies

- C# and .NET 8
- ASP.NET Core MVC
- Entity Framework Core 8
- SQL Server
- ASP.NET Core Identity
- Razor views and Bootstrap

## What you need

The default setup uses Windows. Install:

- .NET 8 SDK
- SQL Server Express LocalDB
- Visual Studio 2022 with the ASP.NET and web development workload, if you want to use Visual Studio

LocalDB is available as a component in Visual Studio Installer.

## How to run the project

### 1. Open the project

Download or clone this repository. Open `AppsChangeLog.sln` in Visual Studio.

To restore the packages from a terminal, run this command from the folder with the solution file:

```powershell
dotnet restore AppsChangeLog.sln
```

### 2. Create the database

The default connection is in `AppsChangeLog/appsettings.json`. It uses `(localdb)\MSSQLLocalDB` and a database called `AppsChangeLog`.

This connection uses your Windows account. You do not need a SQL username or password.

In Visual Studio, open **Tools > NuGet Package Manager > Package Manager Console**. Choose **AppsChangeLog** as the default project. Run:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
Update-Database
```

The project already has a migration. This command creates the database and its tables. You do not need to create a new migration.

### 3. Start the website

Choose the **http** profile next to the Run button in Visual Studio. Then start the project.

You can also start it from a terminal in the solution folder:

```powershell
dotnet run --project AppsChangeLog/AppsChangeLog.csproj --launch-profile http
```

Open http://localhost:5291 in your browser. Keep the terminal open while using the website. Press Ctrl+C to stop it.

### 4. Log in

The app creates a test admin account when it starts in Development mode:

- Username: `root`
- Password: `root`
- You can also log in with the email `root@example.com`.

This account is only for local testing. Do not use it on a public website. If the root account already exists, startup does not change its password.

A new database has no vendors or applications. Start by adding a vendor. Then add an application and select that vendor.

To test vendor access, create a user from **Manage users**. Select a vendor for the user. Their email is also their username.

New users need a password with at least 6 characters. It must have an uppercase letter, a lowercase letter, a number, and a special character.

## Using another SQL Server

You can keep the default LocalDB connection in the project and use another server on your computer.

Right-click the project in Visual Studio and choose **Manage User Secrets**. Add this setting and replace the server name:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER\\YOUR_INSTANCE;Database=AppsChangeLog;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
  }
}
```

The app uses this connection instead of LocalDB in Development mode. User Secrets are not saved in the GitHub repository.

Run `Update-Database` for the selected database before starting the app. Changing the connection does not move data from the old database.

The included launch profiles use Development mode. No API keys or other service accounts are needed for local use.

## Common problems

- If Visual Studio cannot start the https profile, try the http profile. Check the error in the terminal if the app still stops.
- If a build says the application file is locked, stop the running app and build again.
- If the app cannot connect to the database, check that LocalDB or your SQL Server is available. Also check the connection in User Secrets.
- If tables are missing, run `Update-Database` for the database you are using.
