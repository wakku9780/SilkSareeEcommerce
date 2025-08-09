using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                Console.WriteLine("✅ Home/Index called successfully");
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in Home/Index: {ex.Message}");
                throw;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
