using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Services;


namespace SilkSareeEcommerce.Controllers
{
    [Authorize(Roles = "Admin")] // Sirf Admin hi access kare
    public class AdminController : Controller
    {
        private readonly OrderService _orderService;
        private readonly ProductService _productService;
        private readonly UserService _userService;

        public AdminController(OrderService orderService, ProductService productService, UserService userService)
        {
            _orderService = orderService;
            _productService = productService;
            _userService = userService;
        }


        public IActionResult Dashboard()
        {
            return View();
        }


        // ✅ Admin Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // ✅ Show All Orders
        public async Task<IActionResult> Orders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        // ✅ Update Order Status
        [HttpPost]
        [ValidateAntiForgeryToken] // ✅ CSRF protection ke liye
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            await _orderService.UpdateOrderStatusAsync(orderId, status);
            return RedirectToAction("Index");
        }


        // ✅ Show All Products
        public async Task<IActionResult> Products()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // ✅ Show All Users
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }
    }
}
