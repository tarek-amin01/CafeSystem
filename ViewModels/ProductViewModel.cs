using CafeSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace CafeSystem.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string? Description { get; set; }
        
        [Range(0, 100000)]
        public decimal Price { get; set; }
        
        public IFormFile? ImageFile { get; set; }
        
        public string? ExistingImageUrl { get; set; }
        
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        
        [Required]
        public int StockQuantity { get; set; }
        
        public bool IsAvailable { get; set; } = true;
    }
}
