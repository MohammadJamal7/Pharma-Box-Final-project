using Graduation_Project.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SupplierController(ApplicationDbContext context)
        {
             _context = context;
        }
        public async Task<IActionResult> Profile()
        {
            var supplierMedications = await _context.SupplierMedications.ToListAsync();
            return View(supplierMedications);
        }
    }
}
