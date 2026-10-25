using AppsChangeLog.Data;
using AppsChangeLog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppEntity = AppsChangeLog.Models.Application;
using AppUser = AppsChangeLog.Models.User;

namespace AppsChangeLog.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<AppUser> userManager;

        public ApplicationsController(
            ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applications = await context.Applications
                .AsNoTracking()
                .Include(a => a.Vendor)
                .OrderBy(a => a.Name)
                .ToListAsync();

            return View(applications);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var application = await context.Applications
                .AsNoTracking()
                .Include(a => a.Vendor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            if (!await CanAccessApplicationAsync(application.VendorId))
            {
                return NotFound();
            }

            return View(application);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await LoadVendorsAsync();

            return View(new AppEntity
            {
                ReleaseDate = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [Bind("Name,WhatWasChanged,CurrVersion,InstructionsForImport,ReleaseDate,VendorId")]
            AppEntity application)
        {
            await ValidateApplicationAsync(application);

            if (!ModelState.IsValid)
            {
                await LoadVendorsAsync(application.VendorId);
                return View(application);
            }

            application.Name = application.Name.Trim();
            application.WhatWasChanged = application.WhatWasChanged.Trim();
            application.CurrVersion = application.CurrVersion.Trim();
            application.InstructionsForImport = application.InstructionsForImport.Trim();

            application.Vendor = await context.Vendors
                .FirstAsync(v => v.Id == application.VendorId);

            context.Applications.Add(application);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> Edit(int id)
        {
            var application = await context.Applications
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            if (!await CanAccessApplicationAsync(application.VendorId))
            {
                return Forbid();
            }

            await LoadVendorsAsync(application.VendorId);

            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Name,WhatWasChanged,CurrVersion,InstructionsForImport,ReleaseDate,VendorId")]
            AppEntity application)
        {
            var existingApplication = await context.Applications
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existingApplication == null)
            {
                return NotFound();
            }

            if (!await CanAccessApplicationAsync(existingApplication.VendorId))
            {
                return Forbid();
            }

            if (!User.IsInRole("Admin"))
            {
                application.VendorId = existingApplication.VendorId;
            }

            await ValidateApplicationAsync(application);

            if (!ModelState.IsValid)
            {
                application.Id = id;
                await LoadVendorsAsync(application.VendorId);

                return View(application);
            }

            existingApplication.Name = application.Name.Trim();
            existingApplication.WhatWasChanged =
                application.WhatWasChanged.Trim();
            existingApplication.CurrVersion =
                application.CurrVersion.Trim();
            existingApplication.InstructionsForImport =
                application.InstructionsForImport.Trim();
            existingApplication.ReleaseDate = application.ReleaseDate;
            existingApplication.VendorId = application.VendorId;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var application = await context.Applications
                .AsNoTracking()
                .Include(a => a.Vendor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            if (!await CanAccessApplicationAsync(application.VendorId))
            {
                return Forbid();
            }

            return View(application);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var application = await context.Applications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            if (!await CanAccessApplicationAsync(application.VendorId))
            {
                return Forbid();
            }

            context.Applications.Remove(application);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CanAccessApplicationAsync(int vendorId)
        {
            if (User.IsInRole("Admin"))
            {
                return true;
            }

            var user = await userManager.GetUserAsync(User);

            return User.IsInRole("Vendor")
                && user != null
                && user.VendorID == vendorId;
        }

        private async Task LoadVendorsAsync(int? selectedId = null)
        {
            var query = context.Vendors.AsNoTracking();

            if (!User.IsInRole("Admin"))
            {
                var user = await userManager.GetUserAsync(User);

                query = query.Where(v => v.Id == user!.VendorID);
            }

            var vendors = await query
                .OrderBy(v => v.Name)
                .ToListAsync();

            ViewBag.Vendors = new SelectList(
                vendors, "Id", "Name", selectedId);
        }

        private async Task ValidateApplicationAsync(AppEntity application)
        {
            ModelState.Remove(nameof(AppEntity.Vendor));

            if (!User.IsInRole("Admin"))
            {
                ModelState.Remove(nameof(AppEntity.VendorId));
            }

            var vendorExists = await context.Vendors
                .AnyAsync(v => v.Id == application.VendorId);

            if (!vendorExists)
            {
                ModelState.AddModelError(
                    nameof(AppEntity.VendorId),
                    "Select a valid vendor.");
            }

            if (application.ReleaseDate == default)
            {
                ModelState.AddModelError(
                    nameof(AppEntity.ReleaseDate),
                    "Enter a release date.");
            }
        }
    }
}