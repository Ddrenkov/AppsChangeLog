using AppsChangeLog.Data;
using AppsChangeLog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppsChangeLog
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<User, IdentityRole>(options =>
                    {
                        options.User.RequireUniqueEmail = true;
                    })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();

                var userManager = scope.ServiceProvider
                    .GetRequiredService<UserManager<User>>();

                var roleManager = scope.ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole("Admin"));

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                }

                if (!await roleManager.RoleExistsAsync("Vendor"))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole("Vendor"));

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                }

                var root = await userManager.FindByNameAsync("root");

                if (root == null)
                {
                    root = new User
                    {
                        UserName = "root",
                        Email = "root@example.com",
                        FullName = "Root Administrator",
                        EmailConfirmed = true
                    };

                    // Hash this development password without changing other users' rules.
                    root.PasswordHash =
                        userManager.PasswordHasher.HashPassword(root, "root");

                    var result = await userManager.CreateAsync(root);

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                }

                if (!await userManager.IsInRoleAsync(root, "Admin"))
                {
                    var result = await userManager.AddToRoleAsync(root, "Admin");

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                }
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
