﻿using Graduation_Project.Data;
using Graduation_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }




        [HttpGet]
        public IActionResult Register()
        {
            ModelState.Clear(); // Clear any existing validation errors
            return View(new UserRegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
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

                // Create a new user
                var user = new ApplicationUser
                {
                    UserName = model.Email, // Email used as username
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber, // Include phone number
                    Address = model.Address, // Include address
                    FullName = model.FullName,
                    UserType = "Patient" // Specify the user type
                };

                // Create the user with the provided password
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add the default role (e.g., User) to the user
                    await _userManager.AddToRoleAsync(user, "Patient");

                    // Optionally, sign the user in after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to the login page after successful registration
                    return RedirectToAction("Login", "User");
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
        // Display the login form
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // Redirect to the user's profile or dashboard after successful login
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);  // Return the view with the error message
                }
            }

            // If validation fails, redisplay the login form with validation errors
            return View(model);
        }

        // Logout action
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "User");
        }

        [Authorize]
        public async Task<IActionResult> profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _context.Orders.Where(o => o.UserId == user!.Id).ToListAsync();
            var model = new userProfileViewModel
            {
                orders = orders,
                user = user,
            };
            return View(model);
        }

        public async Task<IActionResult> UpdateProfile(string name, string phone, string email, string address)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.FullName = name;
            user.PhoneNumber = phone;
            user.Address = address;

            // Update email using UserManager if it has to be confirmed or requires special handling
            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, email);
                if (!setEmailResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update email.");
                    return View(); // Or handle error as needed
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Profile"); // Redirect to a relevant page
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user); // Return the updated model for review or error display
        }

    }

}
