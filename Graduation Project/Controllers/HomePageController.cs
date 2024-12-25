using Graduation_Project.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.Controllers
{
    public class HomePageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomePageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var groupMedicines = await _context.GroupMedicines.ToListAsync();

            return View(groupMedicines);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
