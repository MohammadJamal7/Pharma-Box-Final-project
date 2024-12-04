using Graduation_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Graduation_Project.Data;

namespace Graduation_Project.Controllers
{
    public class PharmCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PharmCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _context.PharmCarts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Medication)
                .Where(c => c.UserId == userId)
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                cart = new PharmCart { UserId = userId };
                _context.PharmCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return View(cart);
        }

        // Helper method to get the current logged-in user ID
        private async Task<string> GetCurrentUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.Id;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int medicationId, int quantity, string supplierId)
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Quantity must be greater than 0." });
            }

            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            var cart = await _context.PharmCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new PharmCart { UserId = userId };
                _context.PharmCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Check if cart belongs to a different supplier
            if (cart.SupplierId != null && cart.SupplierId != supplierId)
            {
                return Json(new
                {
                    success = false,
                    message = "This cart already contains medications from another supplier. Would you like to clear it and start a new cart?",
                    needsConfirmation = true
                });
            }

            // If the cart is empty or belongs to the same supplier, proceed with adding items
            cart.SupplierId = supplierId;
            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.MedicationId == medicationId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                var medication = await _context.SupplierMedications.FindAsync(medicationId);
                if (medication == null)
                {
                    return Json(new { success = false, message = "Medication not found." });
                }

                var cartItem = new PharmCartItem
                {
                    MedicationId = medicationId,
                    Medication = medication,
                    Quantity = quantity,
                    PharmCartId = cart.Id
                };
                cart.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Medication added to cart." });
        }

        // Remove medication from cart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int medicationId)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            var cart = await _context.PharmCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return Json(new { success = false, message = "Cart not found." });
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.MedicationId == medicationId);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Medication not found in cart." });
            }

            cart.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Medication removed from cart." });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCartAndChangeSupplier(string supplierId, int medicationId, int quantity)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            var cart = await _context.PharmCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                // Remove existing cart items
                _context.PharmCartItems.RemoveRange(cart.CartItems);

                // Reset supplier
                cart.SupplierId = supplierId;
            }

            // Save changes
            await _context.SaveChangesAsync();

            // Now add the new item
            return await AddToCart(medicationId, quantity, supplierId);
        }


        // Checkout and clear cart
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            var currentUser = _context.Users.Include(b => b.Branch).FirstOrDefault(u => u.Id == userId);

            var cart = await _context.PharmCarts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Medication)  // Include the Medication data in CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return Json(new { success = false, message = "Your cart is empty." });
            }

            // Create the SupplierOrder
            var pharmacistOrder = new SupplierOrder
            {
                Pharmacist = cart.User,
                PharmacistId = cart.UserId,
                supplierId = cart.SupplierId,
                orderStatus = "Pending",
                OrderDate = DateTime.Now,
                Branch = currentUser.Branch,
                SupplierOrderItems = new List<SupplierOrderItem>()  // Initialize the list of order items
            };
            Console.WriteLine(pharmacistOrder);
            // Iterate through the CartItems and create SupplierOrderItems
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new SupplierOrderItem
                {
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Medication.Price, // Assuming you have a Price property on SupplierMedication
                    SupplierMedicationId = cartItem.Medication.SupplierMedicationId,
                };

                pharmacistOrder.SupplierOrderItems.Add(orderItem);
            }

            // Add the order to the database
            _context.SupplierOrders.Add(pharmacistOrder);

            // Clear the cart after successful checkout
            cart.CartItems.Clear();
            cart.SupplierId = null;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Checkout successful! Cart cleared." });
        }

        // Clear cart manually
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            var cart = await _context.PharmCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                cart.CartItems.Clear();
                cart.SupplierId = null;
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, message = "Cart cleared." });
        }

        // Get the current cart state (GET method)
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            var cart = await _context.PharmCarts
                .Include(c => c.CartItems)  // Include CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return Json(new { success = false, message = "Cart not found." });
            }

            // Format cart data
            var cartItems = cart.CartItems.Select(ci => new
            {
                MedicationId = ci.MedicationId,
                MedicationName = ci.Medication?.Name,  // Assuming Medication has a Name property
                Quantity = ci.Quantity
            }).ToList();

            return Json(new
            {
                success = true,
                cartItems = cartItems
            });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Quantity must be greater than 0." });
            }

            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            var cart = await _context.PharmCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return Json(new { success = false, message = "Cart not found." });
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Cart item not found." });
            }

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Quantity updated successfully." });
        }

    }
}
