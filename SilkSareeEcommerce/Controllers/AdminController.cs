using Microsoft.AspNetCore.Mvc;

namespace SilkSareeEcommerce.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
