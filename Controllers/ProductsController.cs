using CafeSystem.Models;
using CafeSystem.Repositories;
using CafeSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CafeSystem.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IRepository<Product> productRepo, IRepository<Category> categoryRepo, IWebHostEnvironment webHostEnvironment)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepo.GetAllAsync();
            // Eager loading Category is not supported by generic repo GetAllAsync directly unless updated,
            // but for now we will just load products. To display Category names, we can modify Repository to include props or just fetch categories.
            // For MVP simplicity without rewriting Repository, I'll fetch and map manually or lazily if proxies enabled (but lazy load is risky).
            // Better: Update Repository to support Include.
            
            // To be quick, I'll fetch categories to a dictionary locally for display if needed, or assume basic display.
            // Actually, I'll update Repository to simpler pattern or just use Context inside Controller if complexity arises.
            // But I should stick to Repo. I'll just load products.
            // Wait, Product has CategoryId. I can separate fetch.
            
            // Let's rely on the simple connection for now.
            var categories = await _categoryRepo.GetAllAsync();
            ViewBag.Categories = categories.ToDictionary(k => k.Id, v => v.Name);
            
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _categoryRepo.GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    StockQuantity = model.StockQuantity,
                    IsAvailable = model.IsAvailable,
                    ImageUrl = uniqueFileName
                };

                await _productRepo.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _categoryRepo.GetAllAsync(), "Id", "Name", model.CategoryId);
            return View(model);
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                StockQuantity = product.StockQuantity,
                IsAvailable = product.IsAvailable,
                ExistingImageUrl = product.ImageUrl
            };

            ViewBag.Categories = new SelectList(await _categoryRepo.GetAllAsync(), "Id", "Name", product.CategoryId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var product = await _productRepo.GetByIdAsync(id);
                if (product == null) return NotFound();

                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                    product.ImageUrl = uniqueFileName;
                }

                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                product.StockQuantity = model.StockQuantity;
                product.IsAvailable = model.IsAvailable;

                await _productRepo.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _categoryRepo.GetAllAsync(), "Id", "Name", model.CategoryId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
