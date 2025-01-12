using Graduation_Project.Data;
using Microsoft.AspNetCore.Authorization;
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

        public async Task<IActionResult> Index(int id = 1, int groupId = -1)
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

            // Start with base query for medications
            var query = _context.Medicines
                .Where(g => g.InventoryId == currentBranch.Inventory.InventoryId
                            && g.GroupMedicine != null
                            && g.ImageUrl != null);

            // Apply group filter if a valid group was selected
            if (groupId > 0)
            {
                query = query.Where(m => m.GroupMedicine.GroupMedicineId == groupId);
            }

            // Execute the query
            var publishedMedications = await query.ToListAsync();

            // Pass branch name and selected group ID for display in the view
            ViewBag.BranchName = currentBranch.Name;
            ViewBag.userCurrentBranchId = id; 
            ViewBag.SelectedGroupId = groupId;
            if(groupId != -1)
            {
                var selectedGroup = await _context.GroupMedicines.FirstOrDefaultAsync(g => g.GroupMedicineId == groupId);
                ViewBag.selectedGroup = selectedGroup.Name;
            }
           

            // Return the view with the filtered medications
            return View(publishedMedications);
        }
        public async Task<IActionResult> Branches()
        {
            var branches = await _context.PharmacyBranch.ToListAsync();
            return View(branches);
        }

        // GET: Medicine/Details/5
        public async Task<IActionResult> Details(int id, int branchId)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var medication = await _context.Medicines.Include(m=>m.Inventory)
                .FirstOrDefaultAsync(m => m.MedicineId == id);

            if (medication == null)
            {
                return NotFound();
            }

            var currentBranch = await _context.PharmacyBranch
                .FirstOrDefaultAsync(b => b.BranchId == branchId);

            if (currentBranch == null)
            {
                return NotFound("Branch not found.");
            }

            ViewBag.BranchName = currentBranch.Name;
            ViewBag.userCurrentBranchId = branchId;

            return View(medication);
        }


    }
}
