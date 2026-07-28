using CafeSystem.Models;
using CafeSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CafeSystem.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IRepository<Cart> _cartRepo;
        private readonly IRepository<CartItem> _cartItemRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IRepository<Cart> cartRepo, IRepository<CartItem> cartItemRepo, IRepository<Product> productRepo, UserManager<ApplicationUser> userManager)
        {
            _cartRepo = cartRepo;
            _cartItemRepo = cartItemRepo;
            _productRepo = productRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Need to allow fetching with Include for efficiency, but restricted by generic repo.
            // Will do explicit loading for now.
            var carts = await _cartRepo.FindAsync(c => c.UserId == user.Id);
            var cart = carts.FirstOrDefault();

            if (cart == null)
            {
                cart = new Cart { UserId = user.Id, CartItems = new List<CartItem>() };
                await _cartRepo.AddAsync(cart);
            }
            else
            {
                 // Load items manually since no Include
                 var items = await _cartItemRepo.FindAsync(ci => ci.CartId == cart.Id);
                 cart.CartItems = items.ToList();
                 
                 foreach(var item in cart.CartItems)
                 {
                     item.Product = await _productRepo.GetByIdAsync(item.ProductId);
                 }
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var carts = await _cartRepo.FindAsync(c => c.UserId == user.Id);
            var cart = carts.FirstOrDefault();

            if (cart == null)
            {
                cart = new Cart { UserId = user.Id };
                await _cartRepo.AddAsync(cart);
                // Re-fetch to get Id if not auto-populated (EF usually populates on AddAsync if SaveChanges called)
                // My generic repo calls SaveChangesAsync.
            }

            // Check if item exists in cart
            var cartItems = await _cartItemRepo.FindAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);
            var existingItem = cartItems.FirstOrDefault();

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _cartItemRepo.UpdateAsync(existingItem);
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _cartItemRepo.AddAsync(newItem);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            await _cartItemRepo.DeleteAsync(cartItemId);
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var item = await _cartItemRepo.GetByIdAsync(cartItemId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    await _cartItemRepo.DeleteAsync(cartItemId);
                }
                else
                {
                    item.Quantity = quantity;
                    await _cartItemRepo.UpdateAsync(item);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
