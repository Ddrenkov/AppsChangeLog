# AppsChangeLog

AppsChangeLog keeps the latest information about company apps in one place. Each app has a vendor, version number, release date, changes, and steps for importing it into the company system.

There is no release history. When an app is updated, the old information is replaced.

## What users can do

You must log in to use the website.

Admins can add, edit, and delete apps and vendors. They can also create vendor accounts and delete them.

Vendor users can see and edit their own vendor’s apps. They cannot see apps from other vendors or add and delete apps.

You cannot delete a vendor while it has apps or users. Admin accounts are also protected from deletion.

## Tools used

- C# and .NET 8
- ASP.NET Core MVC
- Entity Framework Core 8
- SQL Server
- ASP.NET Core Identity
- Razor and Bootstrap

## Run the app

You need Windows, .NET 8 SDK, and SQL Server Express LocalDB. LocalDB can be installed from Visual Studio Installer.

Open `AppsChangeLog.sln` in Visual Studio 2022. Visual Studio needs the **ASP.NET and web development** workload.

From the solution folder, restore the packages:

```powershell
dotnet restore AppsChangeLog.sln
```

Next, open **Tools → NuGet Package Manager → Package Manager Console**. Choose **AppsChangeLog** as the default project.

Run these two commands separately:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

```powershell
Update-Database
```

This creates the database using the migration already in the project.

Choose **http** next to the Run button and start the app. The address is [http://localhost:5291](http://localhost:5291).

Or start it from a terminal:

```powershell
dotnet run --project AppsChangeLog/AppsChangeLog.csproj --launch-profile http
```

Leave the terminal open while using the app. Press **Ctrl+C** to stop it.

## First login

The app creates this account in Development mode:

```text
Username: root
Password: root
```

The email `root@example.com` also works for login. This is a local test account, so it should not be used on a public website. Starting the app again does not reset its password.

The database is empty at first. Add a vendor, then add an app for that vendor.

To make a vendor account, open **Manage users**, enter the user’s details, and choose a vendor. The user logs in with their email.

New passwords must have at least 6 characters, including uppercase and lowercase letters, a number, and a special character.

## Database settings

The default connection is in `AppsChangeLog/appsettings.json`. It uses `(localdb)\MSSQLLocalDB` and a database named `AppsChangeLog`. It uses your Windows account, so no SQL password is needed.

For another SQL Server, right-click the project and open **Manage User Secrets**. Add this with your server name:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER\\YOUR_INSTANCE;Database=AppsChangeLog;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
  }
}
```

In Development mode, this setting replaces the default connection. User Secrets stay on your computer and are not uploaded to GitHub.

Run `Update-Database` for the database you choose. Data from the old database will not move automatically.

No API keys or other service accounts are needed.

## If you get an error

- If **https** does not start, try **http**.
- If the app file is locked, stop the app before building again.
- If SQL Server does not connect, check the server and your connection settings.
- If tables are missing, run `Update-Database`.
