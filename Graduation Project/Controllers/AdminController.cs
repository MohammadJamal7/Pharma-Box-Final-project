using Graduation_Project.Data;
using Graduation_Project.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.Controllers
{
    public class AdminController : Controller
    {

        public ApplicationDbContext _context;
        public UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager)
        {
            _context = context;
            _userManager = usermanager;

        }


        public IActionResult Overview()
        {
            var model = new OverViewViewModel
            {
                NumberOfBranches = _context.PharmacyBranch.Count(), // Assuming _context is your DbContext
                NumberOfSuppliers = _context.SupplierMedications.Count(),
                NumberOfUsers = _context.Users.Where(user=>user.UserType=="Patient").Count()
            };

            return View(model);
        }


        public async Task<IActionResult> Branches()
        {
            var branches = await _context.PharmacyBranch.ToListAsync();

            return View(branches);

        }

        [HttpGet]
        public IActionResult AddBranch()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBranch(Branch model)
        {
            if (ModelState.IsValid)
            {
                _context.PharmacyBranch.Add(model);
                await _context.SaveChangesAsync();
                var Inventory = new Inventory
                {

                    BranchId = model.BranchId,

                };
                _context.Inventory.Add(Inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction("Branches");
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> EditBranch(int id)
        {
            var branch = await _context.PharmacyBranch.FirstOrDefaultAsync(b => b.BranchId == id);
            if (branch == null)
            {
                return NotFound();
            }
            return View(branch); // Make sure the correct branch is passed to the view
        }

        [HttpPost]
        public async Task<IActionResult> EditBranch(Branch model)
        {
            if (ModelState.IsValid)
            {
                var oldBranch = await _context.PharmacyBranch.FirstOrDefaultAsync(b => b.BranchId == model.BranchId);
                if (oldBranch == null)
                {
                    return RedirectToAction("Branches");
                }

                oldBranch.Name = model.Name;
                oldBranch.Location = model.Location;
                oldBranch.ContactNumber = model.ContactNumber;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction("Branches");
            }

            return View(model); // If validation fails, return the model back to the view
        }

        public async Task<IActionResult> DeleteBranch(int id)
        {
            // Find the branch to delete, including its inventory.
            var branchToDelete = _context.PharmacyBranch
                .Include(b => b.Inventory)
                .FirstOrDefault(b => b.BranchId == id);

            if (branchToDelete != null)
            {
                // Find the pharmacist associated with the branch.
                var branchPharmacist = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.BranchId == branchToDelete.BranchId);

                // If a pharmacist is found, delete them.
                if (branchPharmacist != null)
                {
                    var deleteUserResult = await _userManager.DeleteAsync(branchPharmacist);
                    if (!deleteUserResult.Succeeded)
                    {
                        // Handle user deletion failure if needed.
                        ModelState.AddModelError("", "Failed to delete pharmacist.");
                        return View(branchToDelete); // Return the view with an error message.
                    }
                }

                // Remove the inventory items related to the branch.
                _context.Inventory.RemoveRange(branchToDelete.Inventory);

                // Remove the branch itself.
                _context.PharmacyBranch.Remove(branchToDelete);

                // Save changes to the database.
                await _context.SaveChangesAsync();
            }

            // Redirect to the Branches page after deletion.
            return RedirectToAction("Branches");
        }



        public async Task<IActionResult> Suppliers()
        {
            var suppliers = await _context.Users.Where(user => user.UserType == "Supplier").ToListAsync();
            return View(suppliers);
        }

        public async Task<IActionResult> DeleteSupplier(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            _context.Users.Remove(user);
            _context.SaveChangesAsync();
            return RedirectToAction("Suppliers");
        }


        public async Task<IActionResult> Admins()
        {
            var Admins = await _context.Users.Where(user => user.UserType == "Pharmacist").Include(user => user.Branch).ToListAsync();
            return View(Admins);
        }


        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                // Handle the case where the user is not found, maybe redirect with an error message
                return NotFound(); // Or redirect with an error message
            }

            _context.Users.Remove(user);

            // Await SaveChangesAsync to ensure the deletion is committed
            await _context.SaveChangesAsync();

            return RedirectToAction("Admins");
        }

        public IActionResult AddAdmin()
        {
            return View();
        }

    }
}
