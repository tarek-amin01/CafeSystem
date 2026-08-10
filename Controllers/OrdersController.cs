using CafeSystem.Models;
using CafeSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CafeSystem.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<OrderItem> _orderItemRepo;
        private readonly IRepository<Cart> _cartRepo;
        private readonly IRepository<CartItem> _cartItemRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(
            IRepository<Order> orderRepo,
            IRepository<OrderItem> orderItemRepo,
            IRepository<Cart> cartRepo,
            IRepository<CartItem> cartItemRepo,
            IRepository<Product> productRepo,
            UserManager<ApplicationUser> userManager)
        {
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _cartRepo = cartRepo;
            _cartItemRepo = cartItemRepo;
            _productRepo = productRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            IEnumerable<Order> orders;
            if (User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("Cashier"))
            {
                orders = await _orderRepo.GetAllAsync();
                // Naive implementation for fetching all, better to have paginated or filtered query.
                // Assuming small scale.
            }
            else
            {
                orders = await _orderRepo.FindAsync(o => o.UserId == user.Id);
            }
            
            // Populate User names manually if needed (Admin view)
            // Ideally Repo should support Include.
            
            return View(orders.OrderByDescending(o => o.OrderDate));
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager") && order.UserId != user.Id)
            {
                return Forbid();
            }

            var items = await _orderItemRepo.FindAsync(oi => oi.OrderId == order.Id);
            order.OrderItems = items.ToList();
            
            foreach(var item in order.OrderItems)
            {
                 item.Product = await _productRepo.GetByIdAsync(item.ProductId);
            }
            
            return View(order);
        }

        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var carts = await _cartRepo.FindAsync(c => c.UserId == user.Id);
            var cart = carts.FirstOrDefault();
            if (cart == null) return RedirectToAction("Index", "Cart");
            
            var items = await _cartItemRepo.FindAsync(ci => ci.CartId == cart.Id);
            if (!items.Any()) return RedirectToAction("Index", "Cart");

            cart.CartItems = items.ToList();
            decimal total = 0;
            foreach(var item in cart.CartItems)
            {
                item.Product = await _productRepo.GetByIdAsync(item.ProductId);
                total += item.Product.Price * item.Quantity;
            }

            ViewBag.Total = total;
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var carts = await _cartRepo.FindAsync(c => c.UserId == user.Id);
            var cart = carts.FirstOrDefault();
            if (cart == null) return RedirectToAction("Index", "Cart");

            var items = await _cartItemRepo.FindAsync(ci => ci.CartId == cart.Id);
            var cartItems = items.ToList();
            if (!cartItems.Any()) return RedirectToAction("Index", "Cart");

            // Calculate total and validate stock
            decimal total = 0;
            foreach (var item in cartItems)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                if (product.StockQuantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Not enough stock for {product.Name}");
                    // Should redirect to Cart with error, but for simplicity here:
                    return RedirectToAction("Index", "Cart"); 
                }
                total += product.Price * item.Quantity;
            }

            // Create Order
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                TotalAmount = total,
                OrderStatus = "Pending",
                PaymentStatus = "Pending"
            };
            await _orderRepo.AddAsync(order);
            // EF Core populates Id after add

            // Create OrderItems and Deduct Stock
            foreach (var item in cartItems)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };
                await _orderItemRepo.AddAsync(orderItem);

                // Deduct stock
                product.StockQuantity -= item.Quantity;
                if (product.StockQuantity == 0) product.IsAvailable = false;
                await _productRepo.UpdateAsync(product);
                
                // Remove from cart
                await _cartItemRepo.DeleteAsync(item.Id);
            }

            return RedirectToAction("Details", new { id = order.Id });
        }

        // Admin/Staff Action
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Cashier")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order != null)
            {
                order.OrderStatus = status;
                if (status == "Completed") order.PaymentStatus = "Paid";
                await _orderRepo.UpdateAsync(order);
            }
            return RedirectToAction("Details", new { id = id });
        }
    }
}
