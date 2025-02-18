using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SilkSareeEcommerce.Controllers
{
    [Authorize]  // User must be logged in to access wishlist
    public class WishlistController : Controller
    {
        private readonly WishlistService _wishlistService;

        public WishlistController(WishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlistItems = await _wishlistService.GetWishlistByUserIdAsync(userId);
            return View(wishlistItems);
        }

        public async Task<IActionResult> Add(int productId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool added = await _wishlistService.AddToWishlistAsync(userId, productId);

            if (added)
                TempData["Success"] = "Product added to wishlist!";
            else
                TempData["Error"] = "Product is already in wishlist!";

            return RedirectToAction("Index", "Product");
        }

        public async Task<IActionResult> Remove(int wishlistId)
        {
            await _wishlistService.RemoveFromWishlistAsync(wishlistId);
            TempData["Success"] = "Product removed from wishlist!";
            return RedirectToAction("Index");
        }
    }
}
