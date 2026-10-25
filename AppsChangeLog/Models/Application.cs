using System.ComponentModel.DataAnnotations;

namespace AppsChangeLog.Models
{
    public class Application
    {
        public int Id { get; set; }

        [Display(Name = "Application name")]
        [Required(ErrorMessage = "Enter the application name.")]
        [MaxLength(100, ErrorMessage = "The application name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "What was changed")]
        [Required(ErrorMessage = "Describe the changes in this version.")]
        public string WhatWasChanged { get; set; } = string.Empty;

        [Display(Name = "Current version")]
        [Required(ErrorMessage = "Enter the current version.")]
        public string CurrVersion { get; set; } = string.Empty;

        [Display(Name = "Import instructions")]
        [Required(ErrorMessage = "Enter the steps for importing this version.")]
        public string InstructionsForImport { get; set; } = string.Empty;

        [Display(Name = "Release date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Vendor")]
        public int VendorId { get; set; }

        public Vendor Vendor { get; set; } = null!;
    }
}
