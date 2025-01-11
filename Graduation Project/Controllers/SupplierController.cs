using Graduation_Project.Data;
using Graduation_Project.Models;
using Graduation_Project.ViewModels;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using System.Diagnostics;

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
        public async Task<IActionResult> Medicines(string id)
        {
            var currrentSupplier = await _userManager.GetUserAsync(User);
            var supplierMedications = await _context.SupplierMedications.Include(s=>s.Supplier).Where(s => s.SupplierId == currrentSupplier.Id).ToListAsync();
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
            return RedirectToAction("Medicines", "Supplier", new { id = supplier.Id });


        }
        [HttpGet]
        [Authorize(Roles = "Supplier")]
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
            
            return RedirectToAction("Medicines", "Supplier", new { id = currentUser.Id });

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
            return RedirectToAction("Medicines", "Supplier", new { id = currentUser.Id });
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
                    return RedirectToAction("OverView", "Supplier", new { id = curretnSupplier.Id });
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


        
        [Authorize(Roles = "Supplier")]
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

            // Update order status to 'Preparing'
            order.orderStatus = "Preparing";

            var userMedications = user.SupplierMedication.ToList();
            var orderMedications = order.SupplierOrderItems.ToList();

            // Update medications stock
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

                    _context.Update(userMedication); // Update medication
                }
            }

            // Update order status
            _context.Update(order);

            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                return Json(new { success = false, message = "An error occurred while saving changes." });
            }

            // Create and save a notification
            var notification = new OrderNotifications
            {
                OrderId = order.Id,
                PharmacistId = order.PharmacistId,
                Message = $"Supplier: {user.FullName} accepted Order #{order.Id}." +
                $" Status: Preparing.",
                NotificationDate = DateTime.Now,
                NotificationType = "Order Accepted"
            };

            _context.OrderNotifications.Add(notification);
            await _context.SaveChangesAsync();

            // Schedule a background job to update the order status to "Delivered" after one day
            BackgroundJob.Schedule(() => UpdateOrderStatusToDelivered(order.Id), TimeSpan.FromDays(1));

            // Return success JSON response
            return Json(new { success = true, orderId = order.Id, orderStatus = order.orderStatus });
        }

        public async Task<IActionResult> RejectOrder(int id)
        {
            var user = await _context.Users
                                     .Include(u => u.SupplierMedication)
                                     .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var order = await _context.SupplierOrders
                                       .FirstOrDefaultAsync(o => o.Id == id);

            if (user == null || order == null)
            {
                return NotFound();
            }

            // Update the order status to 'Rejected'
            order.orderStatus = "Rejected";

            _context.Update(order); // Mark order as updated
            await _context.SaveChangesAsync(); // Save changes

            // Create a notification for the pharmacist
            var notification = new OrderNotifications
            {
                OrderId = order.Id,
                PharmacistId = order.PharmacistId,
                Message = $"Supplier: {user.FullName} rejected Order #{order.Id}. Status: Rejected.",
                NotificationDate = DateTime.Now,
                NotificationType = "Order Rejected"
            };

            // Add the notification to the database
            _context.OrderNotifications.Add(notification);
            await _context.SaveChangesAsync();

            // Return JSON data with the updated order status
            return Json(new { success = true, orderId = order.Id, orderStatus = order.orderStatus });
        }



        public async Task UpdateOrderStatusToPreparing(int orderId)
        {
            var order = await _context.SupplierOrders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null)
            {
                order.orderStatus = "Preparing";
                _context.Update(order);
                await _context.SaveChangesAsync();
            }

        }

        public async Task UpdateOrderStatusToDelivered(int orderId)
        {
            var order = await _context.SupplierOrders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null)
            {
                order.orderStatus = "Delivered";
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
        }
        [Authorize(Roles ="Supplier")]
        public async Task<IActionResult> OverView()
        {
            var user = await _userManager.GetUserAsync(User);
            var medicines = await _context.SupplierMedications.Where(usr=>usr.SupplierId==user!.Id).CountAsync();
            var completedOrders = await _context.SupplierOrders.Where(o=>o.orderStatus =="Delivered").CountAsync();
            var pendingOrders = await _context.SupplierOrders.Where(o=>o.orderStatus =="Preparing").CountAsync();
            var rejectedOrders = await _context.SupplierOrders.Where(o=>o.orderStatus =="Rejected").CountAsync();

            var model = new SupplierOverViewVM
            {
                TotalMedicines = medicines,
                CompletedOrders = completedOrders,
                PendingOrders = pendingOrders,
                RejectedOrders = rejectedOrders
            };
            return View(model);
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();  // Signs out the user
            return RedirectToAction("Login", "Supplier");  // Redirects to the Login action
        }


        [HttpPost]
        public async Task<IActionResult> DeliverOrder(int id)
        {
            var order = await _context.SupplierOrders.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            if (order.orderStatus == "Delivered")
            {
                return Json(new { success = false, message = "Order is already marked as delivered." });
            }

            // Update the order status to "Delivered"
            order.orderStatus = "Delivered";

            try
            {
                await _context.SaveChangesAsync(); // Save the updated order

                // Create a notification for the pharmacist
                var user = await _context.Users
                                          .FirstOrDefaultAsync(u => u.Id == order.PharmacistId); // Get the pharmacist (who delivered the order)

                if (user != null)
                {
                    var notification = new OrderNotifications
                    {
                        OrderId = order.Id,
                        PharmacistId = user.Id,
                        Message = $"Supplier: {user.FullName}: Order #{order.Id} has been marked as Delivered.",
                        NotificationDate = DateTime.Now,
                        NotificationType = "Order Delivered"
                    };

                    // Add the notification to the database
                    _context.OrderNotifications.Add(notification);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the order." });
            }
        }



        public async Task<IActionResult> OrderDetails(int id)
        {
            if (id <= 0) // Check for invalid ID
            {
                return NotFound();
            }

            var order = await _context.SupplierOrders
                .Include(o => o.Pharmacist)
                .Include(o => o.Branch)
                .Include(o => o.SupplierOrderItems)
                    .ThenInclude(oi => oi.SupplierMedication)
                .Where(o => o.Id == id) // Use the correct property name (Id or OrderId)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            // Map to ViewModel
            var orderViewModel = new SupplierOrderViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                orderStatus = order.orderStatus,
                PharmacistName = order.Pharmacist?.FullName, // Null check
                BranchName = order.Branch?.Name, // Null check
                OrderItems = order.SupplierOrderItems.Select(item => new OrderItemViewModel
                {
                    MedicationName = item.SupplierMedication?.Name, // Null check
                    Quantity = item.Quantity,
                    Price = item.Price,
                    
                }).ToList()
            };

            return View(orderViewModel);
        }


    }

}
