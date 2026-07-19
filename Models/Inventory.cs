using System.ComponentModel.DataAnnotations.Schema;

namespace CafeSystem.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        
        public int QuantityChanged { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public string? Remarks { get; set; }
    }
}
