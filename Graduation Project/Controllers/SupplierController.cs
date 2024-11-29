using Graduation_Project.Data;
using Graduation_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public SupplierController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
             _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Profile()
        {
            var supplierMedications = await _context.SupplierMedications.Include(s=>s.Supplier).ToListAsync();
            return View(supplierMedications);
        }
        [HttpGet]
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
               supplierMedication.SupplierId = "0c30650d-ee77-4759-87aa-c87efc96425c";

                // Save the supplier medication to the database
                _context.Add(supplierMedication);
                await _context.SaveChangesAsync();

                // Redirect to a success page or the list of supplier medicines
                return RedirectToAction(nameof(Profile));  // Adjust to appropriate action
            
           
        }
    }
}
