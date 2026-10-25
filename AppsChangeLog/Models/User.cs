using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppsChangeLog.Models
{
    public class User : IdentityUser
    {
        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Enter the full name.")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Vendor")]
        public int? VendorID { get; set; }

        public Vendor? Vendor { get; set; }
    }
}
