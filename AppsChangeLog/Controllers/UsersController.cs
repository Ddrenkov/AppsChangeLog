using System.ComponentModel.DataAnnotations;
using AppsChangeLog.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppUser = AppsChangeLog.Models.User;

namespace AppsChangeLog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users
                .AsNoTracking()
                .Include(u => u.Vendor)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadVendorsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Required, MaxLength(100)] string fullName,
            [Required, EmailAddress, MaxLength(254)] string email,
            [Required] string password,
            [Required] string confirmPassword,
            int vendorId)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError(
                    nameof(confirmPassword),
                    "The passwords do not match.");
            }

            var vendorExists = await context.Vendors
                .AnyAsync(v => v.Id == vendorId);

            if (!vendorExists)
            {
                ModelState.AddModelError(
                    nameof(vendorId),
                    "Select a valid vendor.");
            }

            if (!ModelState.IsValid)
            {
                await LoadVendorsAsync(vendorId);
                return View();
            }

            if (!await roleManager.RoleExistsAsync("Vendor"))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The Vendor role has not been configured.");

                await LoadVendorsAsync(vendorId);
                return View();
            }

            var user = new AppUser
            {
                FullName = fullName.Trim(),
                UserName = email.Trim(),
                Email = email.Trim(),
                VendorID = vendorId
            };

            await using var transaction =
                await context.Database.BeginTransactionAsync();

            var createResult = await userManager.CreateAsync(
                user, password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty, error.Description);
                }

                await LoadVendorsAsync(vendorId);
                return View();
            }

            var roleResult = await userManager.AddToRoleAsync(
                user, "Vendor");

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty, error.Description);
                }

                await LoadVendorsAsync(vendorId);
                return View();
            }

            await transaction.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.Users
                .AsNoTracking()
                .Include(u => u.Vendor)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            if (await CannotDeleteAsync(user))
            {
                return Forbid();
            }

            return View(user);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (await CannotDeleteAsync(user))
            {
                return Forbid();
            }

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty, error.Description);
                }

                return View("Delete", user);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CannotDeleteAsync(AppUser user)
        {
            return user.Id == userManager.GetUserId(User)
                || await userManager.IsInRoleAsync(user, "Admin");
        }

        private async Task LoadVendorsAsync(int? selectedId = null)
        {
            var vendors = await context.Vendors
                .AsNoTracking()
                .OrderBy(v => v.Name)
                .ToListAsync();

            ViewBag.Vendors = new SelectList(
                vendors, "Id", "Name", selectedId);
        }
    }
}