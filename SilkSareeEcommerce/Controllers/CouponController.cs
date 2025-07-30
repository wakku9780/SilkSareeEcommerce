using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Services;

namespace SilkSareeEcommerce.Controllers
{
    [Authorize]
    public class CouponController : Controller
    {
        private readonly CouponService _couponService;
        private readonly CartService _cartService; // Assuming you have a CartService to manage cart operations

        public CouponController(CouponService couponService, CartService cartService)
        {
            _couponService = couponService;
            _cartService = cartService;

        }

        public async Task<IActionResult> Index()
        {
            var coupons = await _couponService.GetAllCouponsAsync();
            return View(coupons);
        }

        public IActionResult Create() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (!ModelState.IsValid)
                return View(coupon);

            await _couponService.CreateCouponAsync(coupon);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string code)
        {
            // Get current cart from session or DB
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cart = await _cartService.GetCartItemsAsync(userId); // Assume this gets cart items
            decimal cartTotal = cart.Sum(item => item.Product.Price * item.Quantity);

            var coupon = await _couponService.ApplyCouponAsync(code, cartTotal);
            if (coupon == null)
            {
                TempData["Error"] = "Invalid or expired coupon!";
                return RedirectToAction("Checkout", "Product");
            }

            decimal discount = cartTotal * (coupon.DiscountPercent / 100);
            HttpContext.Session.SetString("CouponCode", code);
            HttpContext.Session.SetString("DiscountAmount", discount.ToString());

            TempData["Success"] = $"Coupon applied. You saved ₹{discount}!";
            return RedirectToAction("Checkout", "Product");
        }




        //[HttpPost]
        //public async Task<IActionResult> ApplyCoupon(string code, decimal cartTotal)
        //{



        //    var coupon = await _couponService.ApplyCouponAsync(code, cartTotal);
        //    if (coupon == null)
        //    {
        //        TempData["Error"] = "Invalid or expired coupon!";
        //        return RedirectToAction("Checkout", "Product");
        //    }

        //    decimal discount = cartTotal * (coupon.DiscountPercent / 100);
        //    HttpContext.Session.SetString("CouponCode", code);
        //    HttpContext.Session.SetString("DiscountAmount", discount.ToString());

        //    TempData["Success"] = $"Coupon applied. You saved ₹{discount}!";
        //    return RedirectToAction("Checkout", "Product");
        //}

        [HttpPost]
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.Remove("CouponCode");
            HttpContext.Session.Remove("DiscountAmount");
            return RedirectToAction("ViewCart", "Product");
        }



    }

}
