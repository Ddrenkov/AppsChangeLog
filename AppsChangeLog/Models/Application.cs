using System.ComponentModel.DataAnnotations;

namespace AppsChangeLog.Models
{
    public class Application
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string WhatWasChanged { get; set; } = string.Empty;

        [Required]
        public string CurrVersion { get; set; } = string.Empty;

        [Required]
        public string InstructionsForImport { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        public int VendorId { get; set; }

        public Vendor Vendor { get; set; } = null!;
    }
}
