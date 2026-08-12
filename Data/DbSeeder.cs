using CafeSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace CafeSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            // Seed Roles
            var userManager = service.GetService<UserManager<ApplicationUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("Manager"));
            await roleManager.CreateAsync(new IdentityRole("Cashier"));
            await roleManager.CreateAsync(new IdentityRole("Staff"));
            await roleManager.CreateAsync(new IdentityRole("Customer"));

            // Seed Admin
            var admin = new ApplicationUser
            {
                UserName = "admin@cafe.com",
                Email = "admin@cafe.com",
                FullName = "Admin User",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Address = "Cafe HQ"
            };
            
            var userInDb = await userManager.FindByEmailAsync(admin.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }


        public static async Task SeedProductsAsync(IServiceProvider service)
        {
            var context = service.GetService<ApplicationDbContext>();
            
            if (context == null) return;

            // 1. Seed Categories if empty
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Coffee" },
                    new Category { Name = "Bakery" },
                    new Category { Name = "Burgers" },
                    new Category { Name = "Pizza" },
                    new Category { Name = "Drinks" },
                    new Category { Name = "Desserts" },
                    new Category { Name = "Breakfast" },
                    new Category { Name = "Snacks" },
                    new Category { Name = "Combos" }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // 2. Seed Products if empty
            if (!context.Products.Any())
            {
                var categories = context.Categories.ToList();
                var products = new List<Product>();

                // Helper to find Cat ID
                int GetCatId(string name) => categories.FirstOrDefault(c => c.Name == name)?.Id ?? 0;

                // --- COFFEE (8 items) ---
                var coffeeId = GetCatId("Coffee");
                products.AddRange(new[] {
                    new Product { Name = "Signature Espresso", Price = 3.50m, CategoryId = coffeeId, Description = "Rich, full-bodied espresso with a velvety crema.", ImageUrl = "https://images.unsplash.com/photo-1510591509098-f4fdc6d0ff04?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Classic Cappuccino", Price = 4.50m, CategoryId = coffeeId, Description = "Equal parts espresso, steamed milk, and foam.", ImageUrl = "https://images.unsplash.com/photo-1572442388796-11668a67e53d?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Hazelnut Latte", Price = 5.00m, CategoryId = coffeeId, Description = "Espresso with steamed milk and roasted hazelnut syrup.", ImageUrl = "https://images.unsplash.com/photo-1599398054066-e4605dc54f63?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Iced Americano", Price = 4.00m, CategoryId = coffeeId, Description = "Espresso shots topped with cold water and ice.", ImageUrl = "https://images.unsplash.com/photo-1517701604599-bb29b5c5090c?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Caramel Macchiato", Price = 5.50m, CategoryId = coffeeId, Description = "Vanilla syrup, steamed milk, espresso, and caramel drizzle.", ImageUrl = "https://images.unsplash.com/photo-1485808191679-5f8c7c8f37ac?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Flat White", Price = 4.75m, CategoryId = coffeeId, Description = "Smooth ristretto shots with velvety micro-foam.", ImageUrl = "https://images.unsplash.com/photo-1577968897966-3d4325b36b61?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Mocha Delight", Price = 5.25m, CategoryId = coffeeId, Description = "Espresso, bittersweet mocha sauce, and steamed milk.", ImageUrl = "https://images.unsplash.com/photo-1578314675249-a6910f80cc4e?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Cold Brew", Price = 4.50m, CategoryId = coffeeId, Description = "Slow-steeped for 12 hours for a super smooth taste.", ImageUrl = "https://images.unsplash.com/photo-1517701550927-30cf4ba1dba5?auto=format&fit=crop&w=800&q=80" }
                });

                // --- BAKERY (8 items) ---
                var bakeryId = GetCatId("Bakery");
                products.AddRange(new[] {
                    new Product { Name = "Butter Croissant", Price = 3.00m, CategoryId = bakeryId, Description = "Flaky, buttery, and freshly baked every morning.", ImageUrl = "https://images.unsplash.com/photo-1555507036-ab1f4038808a?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Almond Danish", Price = 3.50m, CategoryId = bakeryId, Description = "Filled with almond paste and topped with toasted almonds.", ImageUrl = "https://images.unsplash.com/photo-1509365465985-25d11c17e812?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Blueberry Muffin", Price = 2.75m, CategoryId = bakeryId, Description = "Bursting with fresh blueberries and a crumble top.", ImageUrl = "https://images.unsplash.com/photo-1567234669013-21c65dd34c05?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Chocolate Cookie", Price = 2.50m, CategoryId = bakeryId, Description = "Soft baked with large chunks of Belgian chocolate.", ImageUrl = "https://images.unsplash.com/photo-1499636138143-bd649043ea52?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "New York Bagel", Price = 3.25m, CategoryId = bakeryId, Description = "Toasted bagel with regular or chive cream cheese.", ImageUrl = "https://images.unsplash.com/photo-1585478684894-a95171157806?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Cinnamon Roll", Price = 4.00m, CategoryId = bakeryId, Description = "Warm, gooey cinnamon roll with cream cheese frosting.", ImageUrl = "https://images.unsplash.com/photo-1509365465985-25d11c17e812?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Sourdough Toast", Price = 4.50m, CategoryId = bakeryId, Description = "Avocado smash on artisan sourdough bread.", ImageUrl = "https://images.unsplash.com/photo-1588137372308-15f09a04373e?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Banana Bread", Price = 3.00m, CategoryId = bakeryId, Description = "Moist banana bread slice with walnuts.", ImageUrl = "https://images.unsplash.com/photo-1603569283847-aa295f0d016a?auto=format&fit=crop&w=800&q=80" }
                });

                // --- BURGERS (6 items) ---
                var burgerId = GetCatId("Burgers");
                products.AddRange(new[] {
                    new Product { Name = "Classic Cheeseburger", Price = 10.99m, CategoryId = burgerId, Description = "Beef patty, cheddar, lettuce, tomato, house sauce.", ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Double Bacon Smash", Price = 13.99m, CategoryId = burgerId, Description = "Two smashed patties, crispy bacon, melted cheese.", ImageUrl = "https://images.unsplash.com/photo-1594212699903-ec8a3eca50f5?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Mushroom Swiss", Price = 12.50m, CategoryId = burgerId, Description = "Sautéed mushrooms, swiss cheese, caramelized onions.", ImageUrl = "https://images.unsplash.com/photo-1550547660-d9450f859349?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Beyond Vegan", Price = 14.00m, CategoryId = burgerId, Description = "Plant-based patty, vegan cheese, avocado lime crema.", ImageUrl = "https://images.unsplash.com/photo-1550547660-d9450f859349?auto=format&fit=crop&w=800&q=80" }, /* Using similar burger img */
                    new Product { Name = "Spicy Chicken", Price = 11.50m, CategoryId = burgerId, Description = "Crispy chicken breast, spicy mayo, pickles.", ImageUrl = "https://images.unsplash.com/photo-1615557960916-5f4791effe9d?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "BBQ Ranch Burger", Price = 12.00m, CategoryId = burgerId, Description = "Onion rings, cheddar, BBQ sauce, ranch dressing.", ImageUrl = "https://images.unsplash.com/photo-1596662951482-0c4ba74a6df6?auto=format&fit=crop&w=800&q=80" }
                });
                
                // --- PIZZA (6 items) ---
                var pizzaId = GetCatId("Pizza");
                products.AddRange(new[] {
                    new Product { Name = "Margherita", Price = 12.00m, CategoryId = pizzaId, Description = "San Marzano tomato sauce, fresh mozzarella, basil.", ImageUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Pepperoni Feast", Price = 14.50m, CategoryId = pizzaId, Description = "Loaded with double pepperoni and extra cheese.", ImageUrl = "https://images.unsplash.com/photo-1628840042765-356cda07504e?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Truffle Mushroom", Price = 16.00m, CategoryId = pizzaId, Description = "White base, wild mushrooms, truffle oil, thyme.", ImageUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&w=800&q=80" }, /* Reusing pizza img style */
                    new Product { Name = "BBQ Chicken", Price = 15.00m, CategoryId = pizzaId, Description = "Grilled chicken, red onions, cilantro, BBQ drizzle.", ImageUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Veggie Supreme", Price = 14.00m, CategoryId = pizzaId, Description = "Bell peppers, onions, olives, spinach, mushrooms.", ImageUrl = "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Four Cheese", Price = 13.50m, CategoryId = pizzaId, Description = "Mozzarella, gorgonzola, parmesan, provolone.", ImageUrl = "https://images.unsplash.com/photo-1589187151053-5ec8818e661b?auto=format&fit=crop&w=800&q=80" }
                });

                // --- DRINKS (6 items) ---
                var drinksId = GetCatId("Drinks");
                products.AddRange(new[] {
                    new Product { Name = "Fresh Lemonade", Price = 3.00m, CategoryId = drinksId, Description = "Squeezed fresh daily with a hint of mint.", ImageUrl = "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Berry Smoothie", Price = 5.50m, CategoryId = drinksId, Description = "Strawberries, blueberries, yogurt, and honey.", ImageUrl = "https://images.unsplash.com/photo-1553530666-ba11a90694f3?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Green Detox Juice", Price = 6.00m, CategoryId = drinksId, Description = "Kale, apple, cucumber, lemon, and ginger.", ImageUrl = "https://images.unsplash.com/photo-1610970881699-44a5587cabec?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Mango Lassi", Price = 5.00m, CategoryId = drinksId, Description = "Creamy yogurt drink with sweet mango pulp.", ImageUrl = "https://images.unsplash.com/photo-1623961990059-28356e22bc9e?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Iced Matcha Latte", Price = 5.50m, CategoryId = drinksId, Description = "Premium matcha whisked with milk over ice.", ImageUrl = "https://images.unsplash.com/photo-1515823152108-32183183f4b6?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Sparkling Water", Price = 2.50m, CategoryId = drinksId, Description = "Chilled sparkling water with lime wedge.", ImageUrl = "https://images.unsplash.com/photo-1560023907-5f339617ea30?auto=format&fit=crop&w=800&q=80" }
                });

                // --- DESSERTS (6 items) ---
                var dessertId = GetCatId("Desserts");
                products.AddRange(new[] {
                    new Product { Name = "Cheesecake", Price = 6.50m, CategoryId = dessertId, Description = "New York style cheesecake with berry compote.", ImageUrl = "https://images.unsplash.com/photo-1524351199678-941a58a3df50?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Chocolate Lava Cake", Price = 7.00m, CategoryId = dessertId, Description = "Warm chocolate cake with a molten center.", ImageUrl = "https://images.unsplash.com/photo-1624353365286-3f8d62daad51?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Tiramisu", Price = 6.50m, CategoryId = dessertId, Description = "Classic Italian dessert with espresso and mascarpone.", ImageUrl = "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Fruit Tart", Price = 5.50m, CategoryId = dessertId, Description = "Pastry crust filled with custard and fresh fruit.", ImageUrl = "https://images.unsplash.com/photo-1519915028121-7d3463d20b13?auto=format&fit=crop&w=800&q=80" },
                    new Product { Name = "Brownie Sundae", Price = 6.00m, CategoryId = dessertId, Description = "Warm brownie topped with vanilla ice cream.", ImageUrl = "https://images.unsplash.com/photo-1588632616474-325b597cbe9c?auto=format&fit=crop&w=800&q=80" }, /* Using brownie img */
                    new Product { Name = "Macarons", Price = 8.00m, CategoryId = dessertId, Description = "Assortment of colorful French almond cookies.", ImageUrl = "https://images.unsplash.com/photo-1569864358642-9d1684040f43?auto=format&fit=crop&w=800&q=80" }
                });


                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
