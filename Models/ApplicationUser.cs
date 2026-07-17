using Microsoft.AspNetCore.Identity;

namespace CafeSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? ProfileImagePath { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;
    }
}
