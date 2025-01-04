using Graduation_Project.Data;
using Graduation_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var groupMedicines = await _context.GroupMedicines.ToListAsync();
            var branches = await _context.PharmacyBranch.ToListAsync();
            var model = new HomeViewModel
            {
                branches = branches,
                categories = groupMedicines
            };
            return View(model);
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
