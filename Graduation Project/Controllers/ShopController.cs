using Graduation_Project.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Graduation_Project.Controllers
{
    public class ShopController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ShopController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(int id = 1)
        {
            // Get the current branch (default to branch with ID 1 if none specified)
            var currentBranch = await _context.PharmacyBranch
                .Include(i => i.Inventory)
                .ThenInclude(m => m.Medicines)
                .FirstOrDefaultAsync(b => b.BranchId == id);

            // Handle case where branch does not exist
            if (currentBranch == null)
            {
                return NotFound("Branch not found.");
            }

            // Fetch medications for the current branch
            var publishedMedications = await _context.Medicines
                .Where(g => g.InventoryId == currentBranch.Inventory.InventoryId
                            && g.GroupMedicine != null
                            && g.ImageUrl != null
                            )
                .ToListAsync();

            // Pass branch name for display in the view
            ViewBag.BranchName = currentBranch.Name;

            // Return the view with the medications (can be empty)
            return View(publishedMedications);
        }

        public async Task<IActionResult> Branches()
        {
            var branches = await _context.PharmacyBranch.ToListAsync();
            return View(branches);
        }

    }
}
