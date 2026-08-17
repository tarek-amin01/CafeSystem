using CafeSystem.Models;
using CafeSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CafeSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Category> _categoryRepo;

        public HomeController(ILogger<HomeController> logger, IRepository<Product> productRepo, IRepository<Category> categoryRepo)
        {
            _logger = logger;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var products = await _productRepo.GetAllAsync();
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }
            
            ViewBag.Categories = await _categoryRepo.GetAllAsync();
            ViewBag.CurrentCategory = categoryId;
            
            return View(products);
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();
            
            var category = await _categoryRepo.GetByIdAsync(product.CategoryId);
            product.Category = category;
            
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
