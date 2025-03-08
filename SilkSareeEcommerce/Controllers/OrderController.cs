using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SilkSareeEcommerce.Services;
using Microsoft.AspNetCore.Authorization;

namespace SilkSareeEcommerce.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly CartService _cartService;

        public OrderController(OrderService orderService, CartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var orders = await _orderService.GetUserOrdersAsync(userId);
            return View(orders);
        }


        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string PaymentMethod)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItems = (await _cartService.GetCartItemsAsync(userId)).ToList();
            if (cartItems == null || !cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("ViewCart", "Product");
            }

            if (PaymentMethod == "COD")
            {
                // ✅ Cash on Delivery Order Confirm
                var order = await _orderService.CreateOrderAsync(userId, cartItems, "COD");

                if (order == null)
                {
                    TempData["Error"] = "Failed to place order!";
                    return RedirectToAction("Checkout", "Product");
                }

                // ✅ Order placed successfully, now clear the cart
                await _cartService.ClearCartAsync(userId);

                TempData["Success"] = "Your order has been placed successfully with Cash on Delivery!";
                return RedirectToAction("OrderSuccess");
            }
            else if (PaymentMethod == "PayPal")
            {
                // ✅ Redirect to PayPal Payment Page
                return RedirectToAction("PayWithPayPal", "Payment");
            }

            TempData["Error"] = "Invalid Payment Method!";
            return RedirectToAction("Checkout", "Product");
        }



        [HttpGet]
        public IActionResult OrderSuccess()
        {
            TempData["Success"] = "Your order has been placed successfully!";
            return RedirectToAction("Index", "Home"); // Ya koi aur action
        }

    }
}
