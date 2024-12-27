using Graduation_Project.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.Controllers
{
    public class SupplierOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public SupplierOrdersController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager, RoleManager<IdentityRole> rolemanager, SignInManager<ApplicationUser> signin)
        {
            _context = context;
            _userManager = usermanager;
            _roleManager = rolemanager;
            _signInManager = signin;
        }

      

    }
}
