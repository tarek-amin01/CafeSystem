using System.ComponentModel.DataAnnotations.Schema;

namespace CafeSystem.Models
{
    public class Staff
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        
        public string Position { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }
        
        public DateTime HireDate { get; set; } = DateTime.UtcNow;
    }
}
