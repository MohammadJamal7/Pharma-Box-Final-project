using Graduation_Project.Data;
using Graduation_Project.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var cart = new Cart
                {
                    UserId = user.Id,
                    CreatedAt = DateTime.Now,
                    Items = localStorageCartItems.Select(item => new CartItem
                    {
                        MedicineId = item.MedicineId,
                        Name = item.Name,
                        ImageUrl = item.ImageUrl,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        BranchId = item.BranchId
                    }).ToList()
                };

                _context.Carts.Add(cart); // Assuming `_context` is your DbContext
                await _context.SaveChangesAsync();

                var model = new CheckoutViewModel
                {
                    currentUser = user,
                    Cart = cart
                };

                return View(model);
            }

            return RedirectToAction("Login", "User");
        }

    }
}
