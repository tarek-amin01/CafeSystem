using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CafeSystem.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } // Read-only in view usually

        public string? Address { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfileImage { get; set; }

        public string? CurrentProfileImage { get; set; }
    }
}
