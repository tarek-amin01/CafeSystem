using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        public string? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        public string OrderStatus { get; set; } = "Pending"; 
        
        public string PaymentStatus { get; set; } = "Pending";
        
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
