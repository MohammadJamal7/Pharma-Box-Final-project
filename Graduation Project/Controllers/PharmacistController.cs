using Graduation_Project.Areas.Identity.Pages.Account;
using Graduation_Project.Data;
using Graduation_Project.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.EntityFrameworkCore;
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
            var model = new RegisterVM
            {
                Branches = branches
            };
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
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

                // Create a new user
                var user = new ApplicationUser
                {
                    UserName = model.Email, // Email used as username
                    Email = model.Email,
                    FullName = model.FullName,
                    BranchId = model.BranchId
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
                    return RedirectToAction("Index", "Home");
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



        

    }
}
