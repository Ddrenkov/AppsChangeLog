using AppsChangeLog.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppEntity = AppsChangeLog.Models.Application;

namespace AppsChangeLog.Data
{
    public class ApplicationDbContext: IdentityDbContext<User>
    {
        public ApplicationDbContext(
           DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public DbSet<AppEntity> Applications => Set<AppEntity>();

        public DbSet<Vendor> Vendors => Set<Vendor>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppEntity>()
                .HasOne(a => a.Vendor)
                .WithMany(v => v.Applications)
                .HasForeignKey(a => a.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>()
                .HasOne(u => u.Vendor)
                .WithMany(v => v.Users)
                .HasForeignKey(u => u.VendorID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
