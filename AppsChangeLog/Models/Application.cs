using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace AppsChangeLog.Models
{
    public class Application
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CurrentVersion { get; set; } = string.Empty;

        [Required]
        [MaxLength(5000)]
        public string ChangeLog { get; set; } = string.Empty;

        [Required]
        [MaxLength(10000)]
        public string ImportInstructions { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        public int VendorId { get; set; }

        public Vendor Vendor { get; set; } = null!;
    }
}
