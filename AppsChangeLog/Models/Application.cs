using System.ComponentModel.DataAnnotations;

namespace AppsChangeLog.Models
{
    public class Application
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string WhatWasChanged { get; set; }

        [Required]
        public string CurrVersion { get; set; }

        [Required]
        public string InstructionsForImport { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        public int VendorId { get; set; }

        public Vendor Vendor { get; set; } 
    }
}
