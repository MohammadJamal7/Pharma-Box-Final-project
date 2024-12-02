﻿using Graduation_Project.Data;
using Graduation_Project.Models;
using Graduation_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Profile()
        {
            var supplierMedications = await _context.SupplierMedications.Include(s=>s.Supplier).ToListAsync();
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
                return RedirectToAction(nameof(Profile));  // Adjust to appropriate action
            

           
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
                    // Redirect to the default action (e.g., Home/Index) after successful login
                    return RedirectToAction("Profile", "Supplier");
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
    }
}
