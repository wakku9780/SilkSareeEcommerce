using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SilkSareeEcommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // ✅ Show Cart Items in UI
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account"); // Agar user login nhi hai toh login page pe bhej do

            var cartItems = await _cartService.GetCartItemsAsync(userId);
            return View(cartItems); // Cart ke items ko view me bhej raha hoon
        }

        // ✅ Add Product to Cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            await _cartService.AddToCartAsync(userId, productId, quantity);
            return RedirectToAction("Index"); // Cart page pe redirect
        }

        // ✅ Remove Item from Cart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            await _cartService.RemoveFromCartAsync(userId, productId);
            return RedirectToAction("Index");
        }

        // ✅ Clear Entire Cart
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            await _cartService.ClearCartAsync(userId);
            return RedirectToAction("Index");
        }

        // ✅ Update Quantity in Cart (Increase/Decrease)
        [HttpPost]
        public async Task<IActionResult> UpdateCartQuantity(int productId, string action)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            var result = await _cartService.UpdateCartQuantityAsync(userId, productId, action);
            return RedirectToAction("Index");
        }
    }
}
