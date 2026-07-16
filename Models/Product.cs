using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeSystem.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        public string? Description { get; set; }
        
        [Range(0, 100000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public int StockQuantity { get; set; }
        
        public int CategoryId { get; set; }
        
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        
        public bool IsAvailable { get; set; } = true;
    }
}
