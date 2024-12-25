using Microsoft.AspNetCore.Mvc;

namespace Graduation_Project.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Shop()
        {
            return View();
        }

        public IActionResult Shop_Single()
        {
            return View();
        }
    }
}
