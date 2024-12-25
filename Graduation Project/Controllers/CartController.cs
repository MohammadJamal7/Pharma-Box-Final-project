using Microsoft.AspNetCore.Mvc;

namespace Graduation_Project.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }
    }
}
