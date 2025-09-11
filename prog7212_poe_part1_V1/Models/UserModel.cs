using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace prog7212_poe_part1_V1.Models
{
    public class UserModel : IdentityUser
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
