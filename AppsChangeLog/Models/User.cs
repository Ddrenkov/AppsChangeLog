using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppsChangeLog.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public int? VendorId { get; set; }

        public Vendor? Vendor { get; set; }
    }
}
