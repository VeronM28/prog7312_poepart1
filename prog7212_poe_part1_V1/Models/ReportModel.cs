using System.ComponentModel.DataAnnotations;

namespace prog7212_poe_part1_V1.Models
{
    public class ReportModel
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

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

        [Display(Name = "Image")]
        public string? ImagePath { get; set; }

        [Display(Name = "Date Submitted")]
        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

        // Navigation property
        public virtual UserModel User { get; set; }
    }
}
