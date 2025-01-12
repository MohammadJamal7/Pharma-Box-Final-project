using Graduation_Project.Data;
using Graduation_Project.Models;
using Graduation_Project.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using NuGet.Versioning;
using System.Security.Claims;

namespace Graduation_Project.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CartController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }
        public IActionResult Cart()
        {
            return View();
        }





        [HttpPost]
        public async Task<ActionResult> Checkoutt([FromBody] List<CartItem> localStorageCartItems)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "User");

            var user = await _userManager.GetUserAsync(User);

            // Check if a cart already exists for the user
            var cart = await _context.Carts
                                     .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                // No cart exists, create a new one
                cart = new Cart
                {
                    UserId = user.Id,
                    CreatedAt = DateTime.Now,
                    Items = new List<CartItem>()
                };

                _context.Carts.Add(cart);
            }
            else
            {
                // Clear the existing items before adding new ones
                cart.Items.Clear();
            }

            // Add the items to the cart
            cart.Items.AddRange(localStorageCartItems.Select(item => new CartItem
            {
                MedicineId = item.MedicineId,
                Name = item.Name,
                ImageUrl = item.ImageUrl,
                Price = item.Price,
                Quantity = item.Quantity,
                BranchId = item.BranchId
            }));

            // Save changes
            await _context.SaveChangesAsync();

            // Retrieve branch info (optional)
            var branchId = localStorageCartItems.FirstOrDefault()?.BranchId;
            var branch = branchId.HasValue ? _context.PharmacyBranch.FirstOrDefault(b => b.BranchId == branchId) : null;

            var model = new CheckoutViewModel
            {
                Branch = branch,
                currentUser = user,
                Cart = cart
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(string FullName, string Address, string Email, string PhoneNumber, int CityId, decimal TotalAmount)
        {
            try
            {
                // Get user ID from the claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Assuming NameIdentifier contains user ID

                // Get the user from the database
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);  // Assuming userId is the user's Id

                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("checkoutt", "Cart");
                }

                // Retrieve the user's cart
                var cart = await _context.Carts
                    .Include(c => c.Items)  // Make sure to load the cart items
                    .FirstOrDefaultAsync(c => c.UserId == userId ); // Adjust if you have a CartStatus

                if (cart == null || !cart.Items.Any())
                {
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction("checkoutt", "Cart");
                }

                // Calculate total order amount from cart items
                //var totalAmount = cart.Items.Sum(item => item.Price * item.Quantity);

                // Create the new Order
                var newOrder = new Order
                {
                    Status = "Pending",  // Default status, adjust if needed
                    OrderDate = DateTime.Now,
                    TotalAmount = TotalAmount,
                    BranchId = CityId,  // Map CityId to BranchId (adjust if needed)
                    UserId = user.Id,
                    
                    OrderItems = cart.Items.Select(item => new OrderItem
                    {
                        Quantity = item.Quantity,
                        Price = item.Price,
                        MedicineId = item.MedicineId
                    }).ToList()
                };

                // Add the new order to the database
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

              

                TempData["Success"] = "Your order has been placed successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Log the error (ex) if needed
                TempData["Error"] = "An error occurred while processing your order.";
                return RedirectToAction("checkoutt", "Cart");
            }
        }

    }
}

