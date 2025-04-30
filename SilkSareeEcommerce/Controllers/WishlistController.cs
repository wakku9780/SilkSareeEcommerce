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
        private readonly CartService _cartService;

        public WishlistController(WishlistService wishlistService,CartService cartService)
        {
            _cartService=cartService;
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

        [HttpPost]
        public async Task<IActionResult> Remove(int wishlistId)
        {
            await _wishlistService.RemoveFromWishlistAsync(wishlistId);
            TempData["Success"] = "Product removed from wishlist!";
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearAll()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _wishlistService.ClearWishlistAsync(userId);
            TempData["Success"] = "Your wishlist has been cleared!";
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> MoveToCart(int wishlistId,int quantity)
        {
            // User ka ID lena
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Wishlist se item ko get karna
            var wishlistItem = await _wishlistService.GetWishlistItemByIdAsync(wishlistId);

            // Agar item nahi milta hai to redirect ya error dikhana
            if (wishlistItem == null || wishlistItem.UserId != userId)
            {
                TempData["Error"] = "Item not found or you are not authorized.";
                return RedirectToAction("Index");
            }

            // Cart mein item add karna
            bool addedToCart = await _cartService.AddToCartAsync(userId, wishlistItem.ProductId);

            // Agar item cart mein successfully add ho gaya, to wishlist se remove karna
            if (addedToCart)
            {
                await _wishlistService.RemoveFromWishlistAsync(wishlistId);
                TempData["Success"] = "Item moved to cart successfully!";
            }
            else
            {
                TempData["Error"] = "Item could not be moved to cart!";
            }

            return RedirectToAction("Index");
        }


    }
}
