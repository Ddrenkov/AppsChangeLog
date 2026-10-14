using System.ComponentModel.DataAnnotations;

namespace AppsChangeLog.Models
{
    public class Vendor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(254)]
        public string? ContactEmail { get; set; }

        public ICollection<Application> Applications { get; set; }
            = new List<Application>();

        public ICollection<User> Users { get; set; }
            = new List<User>();

    }
}
