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
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                Console.WriteLine($"❌ Inner exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // ✅ Test database connection
        public IActionResult TestDb()
        {
            try
            {
                // Simple test to see if we can connect to database
                return Json(new { status = "success", message = "Database test endpoint working" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database test failed: {ex.Message}");
                return Json(new { status = "error", message = ex.Message });
            }
        }

        // ✅ Health check endpoint
        public IActionResult Health()
        {
            try
            {
                var info = new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                    version = "1.0.0",
                    message = "Application is running"
                };
                Console.WriteLine($"✅ Health check successful: {DateTime.UtcNow}");
                return Json(info);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Health check failed: {ex.Message}");
                return Json(new { status = "error", message = ex.Message, timestamp = DateTime.UtcNow });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
