using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SilkSareeEcommerce.Services;
using Microsoft.AspNetCore.Authorization;
using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly CartService _cartService;
        private readonly ProductService _productService;


        private readonly UserService _userService;

        public OrderController(OrderService orderService, CartService cartService, ProductService productService, UserService userService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _productService = productService;
            _userService = userService;
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
                    TempData["Error"] = "Failed to place order! due to out of stock";
                    return RedirectToAction("Checkout", "Order");
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
            return RedirectToAction("Checkout", "Order");
        }




        [HttpPost]
        public async Task<IActionResult> PlaceOrderFromBuyNow(BuyNowViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var product = await _productService.GetProductByIdAsync(model.Product.Id);
            if (product == null)
            {
                TempData["Error"] = "Product not found!";
                return RedirectToAction("Index", "Home");
            }

            if (product.Quantity < model.Quantity)
            {
                TempData["Error"] = "Product is out of stock!";
                return RedirectToAction("BuyNow", "Product", new { id = model.Product.Id });
            }

            if (model.PaymentMethod == "COD")
            {
                var order = await _orderService.CreateOrderFromBuyNowAsync(userId, product, model.Quantity, "COD");

                if (order == null)
                {
                    TempData["Error"] = "Failed to place order!";
                    return RedirectToAction("BuyNow", "Product", new { id = model.Product.Id });
                }

                TempData["Success"] = "Your order has been placed successfully!";
                return RedirectToAction("OrderSuccess");
            }
            else if (model.PaymentMethod == "PayPal")
            {
                TempData["BuyNow_ProductId"] = model.Product.Id;
                TempData["BuyNow_Quantity"] = model.Quantity;
                return RedirectToAction("PayWithPayPal", "Payment");
            }

            TempData["Error"] = "Invalid payment method!";
            return RedirectToAction("BuyNow", "Product", new { id = model.Product.Id });
        }



        //[HttpPost]
        //public async Task<IActionResult> PlaceOrderWithAddress(string PaymentMethod, string ShippingAddress, bool SaveAddress)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return Unauthorized();
        //    }

        //    var cartItems = (await _cartService.GetCartItemsAsync(userId)).ToList();
        //    if (cartItems == null || !cartItems.Any())
        //    {
        //        TempData["Error"] = "Your cart is empty!";
        //        return RedirectToAction("ViewCart", "Product");
        //    }

        //    // ✅ Save the shipping address if requested
        //    if (SaveAddress && !string.IsNullOrWhiteSpace(ShippingAddress))
        //    {
        //        await _userService.SaveAddressAsync(userId, ShippingAddress);
        //    }

        //    if (PaymentMethod == "COD")
        //    {
        //        // ✅ Place order with Cash on Delivery
        //        var order = await _orderService.CreateOrderAsync(userId, cartItems, "COD");

        //        if (order == null)
        //        {
        //            TempData["Error"] = "Failed to place order! due to out of stock";
        //            return RedirectToAction("Checkout", "Order");
        //        }

        //        await _cartService.ClearCartAsync(userId);

        //        TempData["Success"] = "Your order has been placed successfully with Cash on Delivery!";
        //        return RedirectToAction("OrderSuccess");
        //    }
        //    else if (PaymentMethod == "PayPal")
        //    {
        //        // ✅ Store shipping address temporarily if needed
        //        TempData["ShippingAddress"] = ShippingAddress;

        //        // ✅ Redirect to PayPal Payment Page
        //        return RedirectToAction("PayWithPayPal", "Payment");
        //    }

        //    TempData["Error"] = "Invalid Payment Method!";
        //    return RedirectToAction("Checkout", "Order");
        //}





        //[HttpPost]
        //public async Task<IActionResult> PlaceOrderWithAddress(string PaymentMethod, string ShippingAddress, bool SaveAddress)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return Unauthorized();
        //    }

        //    var cartItems = (await _cartService.GetCartItemsAsync(userId)).ToList();
        //    if (cartItems == null || !cartItems.Any())
        //    {
        //        TempData["Error"] = "Your cart is empty!";
        //        return RedirectToAction("ViewCart", "Product");
        //    }

        //    // ✅ Save the shipping address if user requested
        //    if (SaveAddress && !string.IsNullOrWhiteSpace(ShippingAddress))
        //    {
        //        await _userService.SaveAddressAsync(userId, ShippingAddress);
        //    }

        //    if (PaymentMethod == "COD")
        //    {
        //        // ✅ Place order with Cash on Delivery
        //        var order = await _orderService.CreateOrderAsync(userId, cartItems, "COD");

        //        if (order == null)
        //        {
        //            TempData["Error"] = "Failed to place order! due to out of stock";
        //            return RedirectToAction("Checkout", "Order");
        //        }

        //        await _cartService.ClearCartAsync(userId);

        //        // ✅ Generate PDF Invoice
        //        var invoicePdf = await _orderService.GenerateOrderInvoiceAsync(order.Id);

        //        // ✅ Return PDF to user as download (or redirect, your choice)
        //        return File(invoicePdf, "application/pdf", $"Order_{order.Id}_Invoice.pdf");

        //        // OR you could store and redirect:
        //        // TempData["Invoice"] = Convert.ToBase64String(invoicePdf);
        //        // return RedirectToAction("OrderSuccess");
        //    }
        //    else if (PaymentMethod == "PayPal")
        //    {
        //        // ✅ Store shipping address temporarily if needed
        //        TempData["ShippingAddress"] = ShippingAddress;

        //        // ✅ Redirect to PayPal Payment Page
        //        return RedirectToAction("PayWithPayPal", "Payment");
        //    }

        //    TempData["Error"] = "Invalid Payment Method!";
        //    return RedirectToAction("Checkout", "Order");
        //}





        [HttpPost]
        public async Task<IActionResult> PlaceOrderWithAddress(string PaymentMethod, string ShippingAddress, bool SaveAddress)
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

            // ✅ Save the shipping address if user requested
            int shippingAddressId = 0;
            if (SaveAddress && !string.IsNullOrWhiteSpace(ShippingAddress))
            {
                var savedAddress = await _userService.SaveAddress1Async(userId, ShippingAddress);
                shippingAddressId = savedAddress.Id; // Address save hone ke baad uska Id assign karo
            }

            if (PaymentMethod == "COD")
            {
                // ✅ Place order with Cash on Delivery
                var order = await _orderService.CreateOrderAsync(userId, cartItems, "COD", shippingAddressId);

                if (order == null)
                {
                    TempData["Error"] = "Failed to place order! due to out of stock";
                    return RedirectToAction("Checkout", "Order");
                }

                await _cartService.ClearCartAsync(userId);

                // ✅ Generate PDF Invoice
                var invoicePdf = await _orderService.GenerateOrderInvoiceAsync(order.Id);

                // ✅ Return PDF to user as download (or redirect, your choice)
                return File(invoicePdf, "application/pdf", $"Order_{order.Id}_Invoice.pdf");
            }
            else if (PaymentMethod == "PayPal")
            {
                // ✅ Store shipping address temporarily if needed
                TempData["ShippingAddress"] = ShippingAddress;

                // ✅ Redirect to PayPal Payment Page
                return RedirectToAction("PayWithPayPal", "Payment");
            }

            TempData["Error"] = "Invalid Payment Method!";
            return RedirectToAction("Checkout", "Order");
        }













        [HttpGet]
        public IActionResult OrderSuccess()
        {
            TempData["Success"] = "Your order has been placed successfully!";
            return RedirectToAction("Index", "Home"); // Ya koi aur action
        }


        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var cartItems = (await _cartService.GetCartItemsAsync(userId)).ToList();
            var savedAddress = await _userService.GetAddressAsync(userId);

            var checkoutViewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = cartItems.Sum(i => i.Quantity * i.Product.Price),
                PaymentMethod = "COD",
                ShippingAddress = savedAddress
            };

            return View(checkoutViewModel);
        }


        //[HttpGet]
        //public async Task<IActionResult> Checkout()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return Unauthorized();
        //    }

        //    var cartItems = (await _cartService.GetCartItemsAsync(userId)).ToList();

        //    var checkoutViewModel = new CheckoutViewModel
        //    {
        //        CartItems = cartItems.ToList(),
        //        TotalAmount = cartItems.Sum(item => item.Quantity * item.Product.Price),
        //        PaymentMethod = "COD" // Default Payment Method
        //    };

        //    return View(checkoutViewModel);

        //    //if (cartItems == null || !cartItems.Any())
        //    //{
        //    //    TempData["Error"] = "Your cart is empty!";
        //    //    return RedirectToAction(nameof(ViewCart));
        //    //}

        //    //return View(cartItems); // ✅ Ye checkout page pe cart items bhejega
        //}

    }
}
