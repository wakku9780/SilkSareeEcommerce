using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Services;
using System.Threading.Tasks;

namespace SilkSareeEcommerce.Controllers
{
    public class UserOrderController : Controller
    {
        private readonly UserOrderService _userOrderService;

        public UserOrderController(UserOrderService userOrderService)
        {
            _userOrderService = userOrderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _userOrderService.GetAllOrdersAsync();
            return View(orders); // Yeh "Index.cshtml" ko render karega
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _userOrderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order); // Yeh "Details.cshtml" ko render karega
        }

        public IActionResult Create()
        {
            return View(); // Yeh "Create.cshtml" ko render karega
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            if (!ModelState.IsValid)
                return View(order);

            await _userOrderService.CreateOrderAsync(order);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _userOrderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order); // Yeh "Edit.cshtml" ko render karega
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (!ModelState.IsValid)
                return View(order);

            await _userOrderService.UpdateOrderAsync(id, order);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _userOrderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order); // Yeh "Delete.cshtml" ko render karega
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userOrderService.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
