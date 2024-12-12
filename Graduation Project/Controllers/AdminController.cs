using Graduation_Project.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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


        public IActionResult Index()
        {
            return View();
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

        public IActionResult DeleteBranch(int id)
        {
            // Your logic for deleting the branch, e.g., find and remove the branch
            var branch = _context.PharmacyBranch.FirstOrDefault(b => b.BranchId == id);
            if (branch != null)
            {
                _context.PharmacyBranch.Remove(branch);
                _context.SaveChanges();
            }
            return RedirectToAction("Branches"); // Redirect to the list or another page after deletion
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
