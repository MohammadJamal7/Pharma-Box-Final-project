using Graduation_Project.Areas.Identity.Pages.Account;
using Graduation_Project.Data;
using Graduation_Project.Models;
using Graduation_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Graduation_Project.Controllers
{
    public class PharmacistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PharmacistController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, UserManager<ApplicationUser> usermanager, RoleManager<IdentityRole> rolemanager, SignInManager<ApplicationUser> signin)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));

            _context = context;
            _userManager = usermanager;
            _roleManager = rolemanager;
            _signInManager = signin;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {

            var branches = await _context.PharmacyBranch.ToListAsync();
            var model = new PharmacistRegister
            {
                Branches = branches
            };

            return View(model);
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
                    // If the email already exists, add a model error
                    ModelState.AddModelError("", "This email is already in use. Please choose another one.");
                    return View(model);
                }

                // Check if passwords match
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View(model);
                }
                var currentBranch = _context.PharmacyBranch.FirstOrDefault(b => b.BranchId == model.BranchId);
                // Create a new user
                var user = new ApplicationUser
                {
                    UserName = model.Email, // Email used as username
                    Email = model.Email,
                    FullName = model.FullName,
                    Branch = currentBranch,
                    BranchId = model.BranchId,
                    UserType = "Pharmacist"
                };

                // Create the user with the provided password
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add the default role (e.g., Pharmacist) to the user
                    await _userManager.AddToRoleAsync(user, "Pharmacist");

                    // Sign the user in
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to the homepage or a desired page after successful registration
                    return RedirectToAction("Login", "Pharmacist");
                }
                else
                {
                    // Add any errors that occurred during the creation process
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // If validation failed, return the same view with validation errors
            return View(model);
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
                    // Redirect to the default action (e.g., Home/Index) after successful login
                    return RedirectToAction("Overview", "Pharmacist");
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


        [Authorize(Roles = "Pharmacist")]

        public async Task<IActionResult> Suppliers()
        {
            var suppliers = await _userManager.Users
                .Where(user => user.UserType == "Supplier").Include(user => user.SupplierMedication).ToListAsync();
            Console.Write(suppliers);
            return View(suppliers);
        }

        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> SupplierMedications(string id)
        {
            var user = await _userManager.Users.Include(user => user.SupplierMedication).FirstOrDefaultAsync(user => user.Id == id);

            return View(user);
        }
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> Branches()
        {
            var branches = await _context.PharmacyBranch.Include(i => i.Inventory).ThenInclude(m => m.Medicines).ToListAsync();
            Console.Write(branches);
            return View(branches);
        }
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> BranchMedications(int id)
        {

            var medicines = await _context.Medicines.Where(m => m.Inventory.BranchId == id).ToListAsync();
            var currentBranch = await _context.PharmacyBranch.FirstOrDefaultAsync(b => b.BranchId == id);
            var branchMedicinesViewModel = new BranchMedicationViewModel
            {
                branch = currentBranch,
                medicines = medicines
            };
            return View(branchMedicinesViewModel);
        }
        [Authorize(Roles = "Pharmacist")]
        public IActionResult Orders()
        {
            return View();
        }
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> PharmacistOrders()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Fetch supplier orders
            var supplierOrders = await _context.SupplierOrders
                .Include(o => o.Pharmacist) // Include Pharmacist to get their details
                .Include(o => o.Branch) // Include Branch to get its details
                .Include(o => o.SupplierOrderItems)
                    .ThenInclude(oi => oi.SupplierMedication) // Include SupplierMedication for details
                .Where(u => u.Pharmacist.Email == currentUser.Email) // Filter by current pharmacist
                .ToListAsync();

            // Fetch supplier details for each order using the supplierId
            var supplierIds = supplierOrders.Select(o => o.supplierId).Distinct().ToList();
            var suppliers = await _context.Users
                .Where(s => supplierIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.FullName);

          

            // Map to view model
            var orderViewModels = supplierOrders.Select(order => new SupplierOrderViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                orderStatus = order.orderStatus,
                PharmacistName = order.Pharmacist.FullName, // Pharmacist's name
                BranchName = order.Branch.Name, // Branch's name
                SupplierId = order.supplierId, // Supplier's ID
                SupplierName = suppliers.TryGetValue(order.supplierId, out var supplierName) ? supplierName : "Unknown", // Supplier's name
                OrderItems = order.SupplierOrderItems.Select(item => new OrderItemViewModel
                {
                    MedicationName = item.SupplierMedication.Name, // Medication name
                    Quantity = item.Quantity, // Quantity ordered
                    Price = item.Price // Price of the item
                }).ToList()
            }).ToList();

            return View(orderViewModels);
        }


        [Authorize(Roles ="Pharmacist")]
        public async Task<IActionResult> Overview()
        {
            var user = await _userManager.GetUserAsync(User);
            var suppliers = await _context.SupplierMedications.CountAsync();
            var branches = await _context.PharmacyBranch.CountAsync();
            var ordersFromUsers = await _context.Orders.Where(o => o.BranchId == user.BranchId).CountAsync();
            var ordersToSuppleirs = await _context.SupplierOrders.Where(o=>o.BranchId==user.BranchId).CountAsync();
            var categories = await _context.GroupMedicines.CountAsync();
            var medications = await _context.Medicines.Where(m => m.Inventory.BranchId == user.BranchId).CountAsync();

            var model = new PharmacistOverViewVM
            {
                Suppliers = suppliers,
                Branches = branches,
                OrdersFromUsers = ordersFromUsers,
                GroupMedicatoins = categories,
                OrdersToSuppliers = ordersToSuppleirs,
                Medications = medications
            };
            //var model = new PharmacistOverViewVM
            //{
            //    Suppliers = 2,
            //    Branches = 3,
            //    OrdersFromUsers = 4,
            //    GroupMedicatoins = 5,
            //    OrdersToSuppliers = 6,
            //    Medications = 7
            //};
            return View(model);
        }

        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> Medications()
        {
            var groups = await _context.GroupMedicines.ToListAsync();
            ViewBag.groups = groups;
            var currentPharmacist = await _userManager.GetUserAsync(User);
            
            var pharmacistMedications = await _context.Medicines.Include(g => g.GroupMedicine).Where(b => b.Inventory.BranchId == currentPharmacist.BranchId).ToListAsync();
            return View(pharmacistMedications);
        }

        [HttpGet]
        [Authorize(Roles = "Pharmacist")]
        public IActionResult CreateMedicine()
        {
            // Assuming _context is your DbContext
            var groups = _context.GroupMedicines
                .Select(g => new SelectListItem
                {
                    Value = g.GroupMedicineId.ToString(), // The ID of the group
                    Text = g.Name // The name of the group
                })
                .ToList();

            ViewBag.GroupMedicineList = groups;
            
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            // Get the order and its related items
            var supplierOrder = await _context.SupplierOrders
                .Include(o => o.SupplierOrderItems)
                    .ThenInclude(oi => oi.SupplierMedication)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (supplierOrder == null || supplierOrder.orderStatus != "Delivered")
            {
                return NotFound("Order not found or not delivered yet.");
            }

            // Get the pharmacist's inventory
            var pharmacistInventory = await _context.Inventory
                .FirstOrDefaultAsync(i => i.BranchId == supplierOrder.BranchId);

            if (pharmacistInventory == null)
            {
                return NotFound("Pharmacist inventory not found.");
            }

            // Process each item in the order
            foreach (var item in supplierOrder.SupplierOrderItems)
            {
                var supplierMedication = item.SupplierMedication;

                if (supplierMedication != null)
                {
                    // Check if the medicine already exists in the pharmacist's inventory
                    var existingMedicine = await _context.Medicines
                        .FirstOrDefaultAsync(m => m.SupplierMedicationId == supplierMedication.SupplierMedicationId);

                    if (existingMedicine != null)
                    {
                        // Update existing medicine (e.g., stock quantity)
                        existingMedicine.ExpiryDate = supplierMedication.ExpiryDate > existingMedicine.ExpiryDate
                            ? supplierMedication.ExpiryDate
                            : existingMedicine.ExpiryDate; // Keep the latest expiry date
                        existingMedicine.StockQuantity += item.Quantity; // Increment stock quantity
                    }
                    else
                    {
                        // Add a new medicine entry
                        var newMedicine = new Medicine
                        {
                            Name = supplierMedication.Name,
                            StockQuantity = supplierMedication.StockQuantity,
                            Description = supplierMedication.Name,
                            HowToUse = "Follow prescription", // Default usage instructions
                            ImageUrl = "/images/product_03.png", // Set if an image is available
                            ExpiryDate = supplierMedication.ExpiryDate,
                            InventoryId = pharmacistInventory.InventoryId,
                            SupplierMedicationId = supplierMedication.SupplierMedicationId,
                            GroupMedicineId = null, // Set group ID if applicable
                            Inventory = pharmacistInventory,
                            Price = (double)supplierMedication.Price,

                        };

                        _context.Medicines.Add(newMedicine);
                    }
                }
            }

            // Mark the order as "Accepted"
            supplierOrder.orderStatus = "Completed";
            await _context.SaveChangesAsync();

            return RedirectToAction("PharmacistOrders", "Pharmacist"); // Redirect to the Orders view after accepting the order
        }

        [HttpPost]
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> RejectOrder(int orderId)
        {
            // Get the order and its related items
            var supplierOrder = await _context.SupplierOrders
                .Include(o => o.SupplierOrderItems)
                    .ThenInclude(oi => oi.SupplierMedication)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (supplierOrder == null || supplierOrder.orderStatus != "Delivered")
            {
                return NotFound("Order not found or not delivered yet.");
            }

            // Process each item in the order
            foreach (var item in supplierOrder.SupplierOrderItems)
            {
                var supplierMedication = item.SupplierMedication;

                if (supplierMedication != null)
                {
                    // Adjust the quantity of the supplier medication to account for the rejected order
                    supplierMedication.StockQuantity += item.Quantity;
                }
            }

            // Mark the order as "Rejected"
            supplierOrder.orderStatus = "Returned";

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("PharmacistOrders", "Pharmacist"); // Redirect to the Orders view after rejecting the order
        }


        [HttpPost]
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> CreateMedicine(MedicineViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentBranch = await _context.PharmacyBranch
                .Include(i => i.Inventory)
                .FirstOrDefaultAsync(b => b.BranchId == currentUser.BranchId);
            var selectedMedicineGroup = await _context.GroupMedicines
                .FirstOrDefaultAsync(g => g.GroupMedicineId == model.GroupMedicineId);
            string uniqueFileName = null;

            // Save the uploaded image to wwwroot/images/medicines
            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/medicines");

                // Ensure the directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique file name
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }
            }

            // Create a new Medicine and save the file path to ImageUrl
            Medicine medicine = new Medicine
            {
                Name = model.Name,
                HowToUse = model.HowToUse,
                StockQuantity = model.StockQuantity,
                ExpiryDate = model.ExpiryDate,
                Description = model.Description,
                GroupMedicine = selectedMedicineGroup,
                GroupMedicineId = model.GroupMedicineId,
                InventoryId = currentBranch.Inventory.InventoryId,
                ImageUrl = uniqueFileName != null ? "/images/medicines/" + uniqueFileName : null,
                RequiresPrescription = model.RequiresPrescription, // Save checkbox value
                Price = model.Price
            };

            // Save to the database
            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            return RedirectToAction("Medications");
        }

        [HttpGet]
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> EditMedicine(int id)
        {
            var groups = _context.GroupMedicines
                .Select(g => new SelectListItem
                {
                    Value = g.GroupMedicineId.ToString(), // The ID of the group
                    Text = g.Name // The name of the group
                })
                .ToList();

            ViewBag.GroupMedicineList = groups;

            var model = _context.Medicines.Include(g => g.GroupMedicine).FirstOrDefault(m => m.MedicineId == id);
            var group = model.GroupMedicine;
            var groupName = "Undefined";
            if (group != null)
            {
                groupName = model.GroupMedicine.Name;
            }

            var viewModel = new MedicineViewModel
            {
                Name = model.Name,
                HowToUse = model.HowToUse,
                StockQuantity = model.StockQuantity,
                ExpiryDate = model.ExpiryDate,
                Description = model.Description,
                GroupMedicineId = model.GroupMedicineId,
                GroupMedicineName = groupName,
                ExistingImagePath = model.ImageUrl,
                RequiresPrescription = model.RequiresPrescription
            };
            return View(viewModel);
        }


        [HttpPost]
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> EditMedicine(int id, MedicineViewModel model)
        {
            var medicine = await _context.Medicines.FirstOrDefaultAsync(m => m.MedicineId == id);

            if (medicine == null)
            {
                return NotFound(); // Handle not found case
            }

            // Update other medicine properties
            medicine.Name = model.Name;
            medicine.HowToUse = model.HowToUse;
            medicine.StockQuantity = model.StockQuantity;
            medicine.ExpiryDate = model.ExpiryDate;
            medicine.Description = model.Description;
            medicine.GroupMedicineId = model.GroupMedicineId;
            medicine.RequiresPrescription = model.RequiresPrescription; // Update RequiresPrescription

            // Handle image upload
            if (model.ImageFile != null)
            {
                // Delete the existing image if it exists
                if (!string.IsNullOrEmpty(medicine.ImageUrl) && !medicine.ImageUrl.Contains("default-medicine.jpg"))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, medicine.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save the new image
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/medicines");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                // Update the ImageUrl property
                medicine.ImageUrl = "/images/medicines/" + uniqueFileName;
            }

            // Save the updated data to the database
            _context.Medicines.Update(medicine);
            await _context.SaveChangesAsync();

            return RedirectToAction("Medications"); // Redirect to the relevant page
        }


        [HttpPost]
        [Authorize(Roles = "Pharmacist")]
        public IActionResult DeleteMedicine(int id)
        {
            // Your logic to delete the medicine entry
            var medicine = _context.Medicines.Find(id);
            if (medicine == null)
            {
                return Json(new { success = false, message = "Medicine not found." });
            }

            _context.Medicines.Remove(medicine);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> GroupsOfMedicines()
        {
            var groups = await _context.GroupMedicines.Include(g => g.Medicines).ToListAsync();
            return View(groups);
        }

        [HttpGet]
        [Authorize(Roles = "Pharmacist")]
        public IActionResult AddGroupMed()
        {
            return View();
        }

        [Authorize(Roles = "Pharmacist")]
        [HttpPost]
        public async Task<IActionResult> AddGroupMed(string Name, IFormFile Image)
        {
            if (!string.IsNullOrWhiteSpace(Name) && Image != null)
            {
                // Step 1: Handle file upload
                var fileExtension = Path.GetExtension(Image.FileName);  // Get file extension
                var fileName = Path.GetFileNameWithoutExtension(Image.FileName);  // Get file name without extension
                var newFileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + fileExtension;  // Create a unique file name

                // Path to save the image (wwwroot/images folder)
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", newFileName);

                // Step 2: Save the file to disk
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }

                // Step 3: Create and save the new GroupMedicine object to the database
                var groupMedicine = new GroupMedicine
                {
                    Name = Name,
                    ImageUrl = "/images/" + newFileName  // Save the relative path of the image
                };

                _context.GroupMedicines.Add(groupMedicine);
                await _context.SaveChangesAsync();

                // Step 4: Redirect or return success message
                return RedirectToAction("GroupsOfMedicines");  // Redirect to a page that shows the list of group medicines
            }

            // If validation fails, show the same form with an error message
            ViewBag.ErrorMessage = "Please fill in all fields.";
            return View();
        }


        [HttpGet]
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> EditGroup(int id)
        {
            var groupMedicine = await _context.GroupMedicines.FindAsync(id);
            if (groupMedicine == null)
            {
                return NotFound();
            }

            return View(groupMedicine);
        }

       
        [HttpPost]
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> EditGroup(int id, string Name, IFormFile Image)
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                var groupMedicine = await _context.GroupMedicines.FindAsync(id);
                if (groupMedicine == null)
                {
                    return NotFound();
                }

                // Update the group name
                groupMedicine.Name = Name;

                // If a new image is uploaded, save the new image
                if (Image != null)
                {
                    // Delete the old image file (if exists)
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", groupMedicine.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    // Save the new image
                    var fileExtension = Path.GetExtension(Image.FileName);
                    var fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                    var newFileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + fileExtension;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", newFileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }

                    // Update the ImagePath in the database
                    groupMedicine.ImageUrl = "/images/" + newFileName;
                }

                // Save changes to the database
                _context.Update(groupMedicine);
                await _context.SaveChangesAsync();

                // Redirect to the list or details page
                return RedirectToAction("GroupsOfMedicines");  // Or redirect to a specific page after editing
            }

            // If validation fails, show the same form with an error message
            ViewBag.ErrorMessage = "Please fill in all fields.";
            return View();
        }
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.GroupMedicines.FindAsync(id);
            _context.GroupMedicines.Remove(group);
            await _context.SaveChangesAsync();
            return RedirectToAction("GroupsOfMedicines");
        }



        [Authorize(Roles = "Pharmacist")]
        public IActionResult Inventory()
        {
            return View();
        }

        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> Notifications()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Fetch notifications for the current pharmacist, including related SupplierOrder details
            var notifications = await _context.OrderNotifications
                .Include(n => n.SupplierOrder) // Include related SupplierOrder details
                .Where(n => n.PharmacistId == currentUser.Id)
                .OrderByDescending(n => n.NotificationDate) // Most recent notifications first
                .ToListAsync(); // Asynchronously load data

            var test=3;
            return View(notifications); // Pass the notifications directly to the view
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            // Find the notification by ID
            var notification = await _context.OrderNotifications
                                              .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
            {
                return Json(new { success = false, message = "Notification not found." });
            }

            // Remove the notification
            _context.OrderNotifications.Remove(notification);
            await _context.SaveChangesAsync();

            // Return a success response
            return Json(new { success = true });
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();  // Signs out the user
            return RedirectToAction("Login", "Pharmacist");  // Redirects to the Login action
        }


    }
}
