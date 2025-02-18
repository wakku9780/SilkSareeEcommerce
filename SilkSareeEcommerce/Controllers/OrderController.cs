using Microsoft.AspNetCore.Mvc;

namespace SilkSareeEcommerce.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
