﻿using Graduation_Project.Areas.Identity.Pages.Account;
using Graduation_Project.Data;
using Graduation_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Graduation_Project.Controllers
{
    public class PharmacistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public PharmacistController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager, RoleManager<IdentityRole> rolemanager, SignInManager<ApplicationUser> signin) {

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
                    return RedirectToAction("Profile", "Pharmacist");
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
            return RedirectToAction("Login", "Pharmacist");
        }


        [Authorize(Roles ="Pharmacist")]
        public async Task<IActionResult> SuppliersDetails()
        {
            var suppliers = await _userManager.Users
                .Where(user => user.UserType == "Supplier").Include(user=>user.SupplierMedication).ToListAsync();
            Console.Write(suppliers);
            return View(suppliers);
        }


        public async Task<IActionResult> SupplierMedications(string id)
        {
            var user = await _userManager.Users.Include(user=>user.SupplierMedication).FirstOrDefaultAsync(user => user.Id == id);
            
            return View(user);
        }

        public async Task<IActionResult> PharmacistOrders()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var supplierOrders = await _context.SupplierOrders
          .Include(o => o.Pharmacist).Where(u=>u.Pharmacist.Email == currentUser.Email)  // Include the Pharmacist (ApplicationUser) to get the name
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

        public IActionResult Profile()
        {
            return View();
        }

        public async Task<IActionResult> Medications()
        {
                var pharmacistMedications = await _context.Medicines.ToListAsync();
                return View(pharmacistMedications);
            
        }

    }


}

