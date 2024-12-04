using Graduation_Project.Data;
using Graduation_Project.Models;
using Graduation_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Identity.Client;

namespace Graduation_Project.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public SupplierController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signin)
        {
             _context = context;
            _userManager = userManager;
            _signInManager = signin;
        }
        [Authorize(Roles = "Supplier")]
        public async Task<IActionResult> Profile(string id)
        {
            var supplierMedications = await _context.SupplierMedications.Include(s=>s.Supplier).Where(s => s.SupplierId == id).ToListAsync();
            return View(supplierMedications);
        }
        [HttpGet]
        [Authorize(Roles = "Supplier")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: Create medicine
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierMedication supplierMedication)
        {
           
                // Get the currently logged-in supplier (the user who is authenticated)
                var supplier = await _userManager.GetUserAsync(User);
           
                // Set the supplier's ID to the SupplierId field of the SupplierMedication model
                supplierMedication.SupplierId = supplier.Id;

                // Save the supplier medication to the database
                _context.Add(supplierMedication);
                await _context.SaveChangesAsync();

            // Redirect to a success page or the list of supplier medicines
            return RedirectToAction("Profile", "Supplier", new { id = supplier.Id });


        }
        [HttpGet]
        public async Task <IActionResult> Edit(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            // Retrieve the medicine to edit based on the id
            var medicineToEdit = _context.SupplierMedications
                                         .FirstOrDefault(m => m.SupplierMedicationId == id);
           
            return View(medicineToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,SupplierMedication model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var existingMedication = await _context.SupplierMedications
            .FirstOrDefaultAsync(m => m.SupplierMedicationId == id && m.SupplierId == currentUser.Id);

                // Update the existing medication properties
                existingMedication.Name = model.Name;
                existingMedication.ExpiryDate = model.ExpiryDate;
                existingMedication.Price = model.Price;
                existingMedication.StockQuantity = model.StockQuantity;
            
                _context.Update(existingMedication);
                await _context.SaveChangesAsync();
            
            return RedirectToAction("Profile", "Supplier", new { id = currentUser.Id });

        }

        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Fetch the existing medication record from the database
            var medicationToDelete = await _context.SupplierMedications
                .FirstOrDefaultAsync(m => m.SupplierMedicationId == id && m.SupplierId == currentUser.Id);

            if (medicationToDelete == null)
            {
                // Handle the case when the medication is not found (optional)
                return NotFound();
            }

            // Remove the medication from the context
            _context.SupplierMedications.Remove(medicationToDelete);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the supplier profile
            return RedirectToAction("Profile", "Supplier", new { id = currentUser.Id });
        }


        [HttpGet]
        public IActionResult Register()
        {
            var model = new PharmacistRegister();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(PharmacistRegister model)
        {
            if (true)
            {
                // Check if the email already exists in the system
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "This email is already in use. Please choose another one.");
                    return View(model);
                }

                // Check if passwords match
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View(model);
                }

                // Validate password format
                if (!IsValidPassword(model.Password))
                {
                    ModelState.AddModelError("", "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
                    return View(model);
                }

                // Create a new user (for supplier)
                var user = new ApplicationUser
                {
                    UserName = model.Email, // Email used as username
                    Email = model.Email,
                    FullName = model.FullName,
                    UserType = "Supplier"
                };

                // Create the user with the provided password
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign the "Supplier" role to the user
                    var roleResult = await _userManager.AddToRoleAsync(user, "Supplier");

                    if (!roleResult.Succeeded)
                    {
                        // Handle any errors related to assigning the role
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }

                    // Sign the user in
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to the login page or another desired page
                    return RedirectToAction("Login", "Supplier");
                }
                else
                {
                    // Handle errors during user creation
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // Return the view if validation failed
            return View(model);
        }

        // Helper method to validate password format
        private bool IsValidPassword(string password)
        {
            // Password must be at least 8 characters long, contain at least one uppercase letter,
            // one lowercase letter, one number, and one special character.
            var hasUpperCase = password.Any(c => char.IsUpper(c));
            var hasLowerCase = password.Any(c => char.IsLower(c));
            var hasDigit = password.Any(c => char.IsDigit(c));
            var hasSpecialChar = password.Any(c => "!@#$%^&*()_+[]{}|;:,.<>?".Contains(c));

            return password.Length >= 8 && hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(PharmacistLogin model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var curretnSupplier = await _userManager.FindByEmailAsync(model.Email);  // Retrieve supplier ID

                    // Redirect to the default action (e.g., Home/Index) after successful login
                    return RedirectToAction("Profile", "Supplier", new { id = curretnSupplier.Id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Supplier");
        }

        // GET: /Supplier/Orders
        public async Task<IActionResult> DisplayOrders()
        {
            var supplierOrders = await _context.SupplierOrders
          .Include(o => o.Pharmacist)  // Include the Pharmacist (ApplicationUser) to get the name
          .Include(o => o.Branch)  // Include the Branch to get the branch name
          .Include(o => o.SupplierOrderItems)
              .ThenInclude(oi => oi.SupplierMedication) // Include Medication to get the name and price
          .ToListAsync();

            var orderViewModels = supplierOrders.Select(order => new SupplierOrderViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                orderStatus = order.orderStatus,
                PharmacistName = order.Pharmacist.FullName, // Assuming UserName contains the pharmacist name
                BranchName = order.Branch.Name,  // Assuming Branch has a Name property
                OrderItems = order.SupplierOrderItems.Select(item => new OrderItemViewModel
                {
                    MedicationName = item.SupplierMedication.Name, // Assuming Name is a property of SupplierMedication
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            }).ToList();

            return View(orderViewModels);
        }

        public async Task<IActionResult> AcceptOrder(int id)
        {
            var user = await _context.Users
                                     .Include(u => u.SupplierMedication)
                                     .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var order = await _context.SupplierOrders
                                       .Include(o => o.SupplierOrderItems)
                                       .FirstOrDefaultAsync(o => o.Id == id);

            if (user == null || order == null)
            {
                return NotFound();
            }

            order.orderStatus = "Confirmed"; // Update order status to Confirmed
            var userMedications = user.SupplierMedication.ToList();
            var orderMedications = order.SupplierOrderItems.ToList();

            // Update medications
            foreach (var orderItem in orderMedications)
            {
                var userMedication = userMedications.FirstOrDefault(med => med.SupplierMedicationId == orderItem.SupplierMedicationId);

                if (userMedication != null)
                {
                    userMedication.StockQuantity -= orderItem.Quantity;

                    if (userMedication.StockQuantity < 0)
                    {
                        ModelState.AddModelError("", $"Not enough stock for {userMedication.Name}. Available: {userMedication.StockQuantity}, Required: {orderItem.Quantity}");
                        return Json(new { success = false, message = "Not enough stock." });
                    }

                    _context.Update(userMedication); // Mark medication as updated
                }
            }

            _context.Update(order); // Mark order as updated
            await _context.SaveChangesAsync(); // Save all changes

            // Return JSON data with the updated order status
            return Json(new { success = true, orderId = order.Id, orderStatus = order.orderStatus });
        }



    }

}
