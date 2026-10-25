using System.Diagnostics;
using AppsChangeLog.Data;
using AppsChangeLog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppsChangeLog.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;

        private readonly UserManager<User> userManager;

        public HomeController(ApplicationDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var query = context.Applications.AsNoTracking();

            if (!User.IsInRole("Admin"))
            {
                var user = await userManager.GetUserAsync(User);

                if (!User.IsInRole("Vendor") || user?.VendorID == null)
                {
                    return Forbid();
                }

                query = query.Where(a => a.VendorId == user.VendorID.Value);
            }

            var applications = await query
                .Include(a => a.Vendor)
                .OrderBy(a => a.Name)
                .ToListAsync();

            return View(applications);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}