using AppsChangeLog.Data;
using AppsChangeLog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppsChangeLog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VendorsController : Controller
    {
        private readonly ApplicationDbContext context;

        public VendorsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vendors = await context.Vendors
                .AsNoTracking()
                .OrderBy(v => v.Name)
                .ToListAsync();

            return View(vendors);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var vendor = await context.Vendors
                .AsNoTracking()
                .Include(v => v.Applications)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vendor == null)
            {
                return NotFound();
            }

            return View(vendor);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Vendor());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,ContactEmail")] Vendor vendor)
        {
            if (!ModelState.IsValid)
            {
                return View(vendor);
            }

            var newVendor = new Vendor
            {
                Name = vendor.Name.Trim(),
                ContactEmail = vendor.ContactEmail?.Trim()
            };

            context.Vendors.Add(newVendor);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vendor = await context.Vendors
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vendor == null)
            {
                return NotFound();
            }

            return View(vendor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Name,ContactEmail")] Vendor vendor)
        {
            var existingVendor = await context.Vendors.FindAsync(id);

            if (existingVendor == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                vendor.Id = id;
                return View(vendor);
            }

            existingVendor.Name = vendor.Name.Trim();
            existingVendor.ContactEmail = vendor.ContactEmail?.Trim();

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vendor = await context.Vendors
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vendor == null)
            {
                return NotFound();
            }

            return View(vendor);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendor = await context.Vendors.FindAsync(id);

            if (vendor == null)
            {
                return NotFound();
            }

            var hasApplications = await context.Applications
                .AnyAsync(a => a.VendorId == id);

            var hasUsers = await context.Users
                .AnyAsync(u => u.VendorID == id);

            if (hasApplications || hasUsers)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Cannot delete a vendor that has applications or users.");

                return View("Delete", vendor);
            }

            context.Vendors.Remove(vendor);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
