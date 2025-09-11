using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace prog7212_poe_part1_V1.ViewModels
{
    public class ReportViewModel
    {
        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Required]
        [Display(Name = "Description")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Location")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
        public string Location { get; set; }

        [Display(Name = "Attach Image")]
        public IFormFile? ImageFile { get; set; }

        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
