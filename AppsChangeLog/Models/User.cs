using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppsChangeLog.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        public int? VendorID { get; set; }

        public Vendor? Vendor { get; set; }
    }
}
