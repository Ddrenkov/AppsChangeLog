using System.ComponentModel.DataAnnotations;

namespace AppsChangeLog.Models
{
    public class Vendor
    {
        public int Id { get; set; }

        [Display(Name = "Vendor name")]
        [Required(ErrorMessage = "Enter the vendor name.")]
        [MaxLength(100, ErrorMessage = "The vendor name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Contact email")]
        [EmailAddress(ErrorMessage = "Enter a valid contact email address.")]
        public string? ContactEmail { get; set; }

        public ICollection<Application> Applications { get; set; }
            = new List<Application>();

        public ICollection<User> Users { get; set; }
            = new List<User>();

    }
}
