using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SilkSareeEcommerce.Services;
using Microsoft.AspNetCore.Authorization;
using SilkSareeEcommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace SilkSareeEcommerce.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly CartService _cartService;
        private readonly ProductService _productService;
        private readonly UserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            OrderService orderService, 
            CartService cartService, 
            ProductService productService, 
            UserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _cartService = cartService;
            _productService = productService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
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

                // ✅ Send order confirmation email
                try
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var shippingAddress = await _userService.GetAddressAsync(userId);
                        var addressText = shippingAddress?.Address ?? "Address not specified";
                        
                        var emailSent = await _orderService.SendOrderConfirmationEmailAsync(order, user, addressText);
                        
                        if (emailSent)
                        {
                            _logger.LogInformation($"Order confirmation email sent successfully for order #{order.Id}");
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to send order confirmation email for order #{order.Id}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error sending order confirmation email for order #{order.Id}: {ex.Message}");
                    // Don't fail the order if email fails
                }

                TempData["Success"] = "Your order has been placed successfully with Cash on Delivery! Check your email for confirmation.";
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
        public async Task<IActionResult> PlaceOrderWithAddress(CheckoutViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var cartItems = (await _cartService.GetCartItemsAsync(userId)).ToList();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("ViewCart", "Product");
            }

            // 🛠️ Fixed logic: Check priority - 1) SelectedSavedAddress 2) ExistingAddress 3) ShippingAddress
            string finalShippingAddress = !string.IsNullOrWhiteSpace(model.SelectedSavedAddress)
                ? model.SelectedSavedAddress
                : !string.IsNullOrWhiteSpace(model.ExistingAddress)
                    ? model.ExistingAddress
                    : model.ShippingAddress;

            if (string.IsNullOrWhiteSpace(finalShippingAddress))
            {
                TempData["Error"] = "Please provide or select a shipping address.";
                return RedirectToAction("Checkout", "Product");
            }

            int shippingAddressId = 0;

            if (!string.IsNullOrWhiteSpace(model.SelectedSavedAddress))
            {
                // 🟢 Convert selected saved address ID string to int
                int.TryParse(model.SelectedSavedAddress, out shippingAddressId);
            }
            else if (model.SaveAddress && !string.IsNullOrWhiteSpace(model.ShippingAddress))
            {
                var savedAddress = await _userService.SaveAddress1Async(userId, model.ShippingAddress);
                shippingAddressId = savedAddress.Id;
            }


            if (model.PaymentMethod == "COD")
            {
                var order = await _orderService.CreateOrderAsync(userId, cartItems, "COD", shippingAddressId);
                if (order == null)
                {
                    TempData["Error"] = "Failed to place order due to out of stock.";
                    return RedirectToAction("Checkout", "Product");
                }

                await _cartService.ClearCartAsync(userId);

                // ✅ Send order confirmation email
                try
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var emailSent = await _orderService.SendOrderConfirmationEmailAsync(order, user, finalShippingAddress);
                        
                        if (emailSent)
                        {
                            _logger.LogInformation($"Order confirmation email sent successfully for order #{order.Id}");
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to send order confirmation email for order #{order.Id}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error sending order confirmation email for order #{order.Id}: {ex.Message}");
                    // Don't fail the order if email fails
                }

                var invoicePdf = await _orderService.GenerateOrderInvoiceAsync(order.Id);
                var invoiceDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "invoices");
                if (!Directory.Exists(invoiceDir)) Directory.CreateDirectory(invoiceDir);

                var fileName = $"Order_{order.Id}_Invoice.pdf";
                var fullPath = Path.Combine(invoiceDir, fileName);
                System.IO.File.WriteAllBytes(fullPath, invoicePdf);

                TempData["Success"] = "Your order has been placed successfully with Cash on Delivery! Check your email for confirmation.";
                TempData["InvoiceFile"] = "/invoices/" + fileName;

                return RedirectToAction("OrderSuccess");
            }
            else if (model.PaymentMethod == "PayPal")
            {
                TempData["ShippingAddress"] = finalShippingAddress;
                return RedirectToAction("PayWithPayPal", "Payment");
            }

            TempData["Error"] = "Invalid Payment Method!";
            return RedirectToAction("Checkout", "Order");
        }





        //important2
        //[HttpPost]
        //public async Task<IActionResult> PlaceOrderWithAddress(CheckoutViewModel model)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userId)) return Unauthorized();

        //    var cartItems = (await _cartService.GetCartItemsAsync(userId)).ToList();
        //    if (!cartItems.Any())
        //    {
        //        TempData["Error"] = "Your cart is empty!";
        //        return RedirectToAction("ViewCart", "Product");
        //    }

        //    string finalShippingAddress = !string.IsNullOrWhiteSpace(model.ExistingAddress)
        //        ? model.ExistingAddress
        //        : model.ShippingAddress;

        //    if (string.IsNullOrWhiteSpace(finalShippingAddress))
        //    {
        //        TempData["Error"] = "Please provide or select a shipping address.";
        //        return RedirectToAction("Checkout", "Product");
        //    }

        //    int shippingAddressId = 0;
        //    if (model.SaveAddress)
        //    {
        //        var savedAddress = await _userService.SaveAddress1Async(userId, finalShippingAddress);
        //        shippingAddressId = savedAddress.Id;
        //    }

        //    if (model.PaymentMethod == "COD")
        //    {
        //        var order = await _orderService.CreateOrderAsync(userId, cartItems, "COD", shippingAddressId);
        //        if (order == null)
        //        {
        //            TempData["Error"] = "Failed to place order due to out of stock.";
        //            return RedirectToAction("Checkout", "Product");
        //        }

        //        await _cartService.ClearCartAsync(userId);

        //        // ✅ Generate invoice PDF
        //        var invoicePdf = await _orderService.GenerateOrderInvoiceAsync(order.Id);

        //        // ✅ Save PDF to wwwroot/invoices
        //        var invoiceDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "invoices");
        //        if (!Directory.Exists(invoiceDir)) Directory.CreateDirectory(invoiceDir);

        //        var fileName = $"Order_{order.Id}_Invoice.pdf";
        //        var fullPath = Path.Combine(invoiceDir, fileName);
        //        System.IO.File.WriteAllBytes(fullPath, invoicePdf);

        //        TempData["Success"] = "Your order has been placed successfully with Cash on Delivery!";
        //        TempData["InvoiceFile"] = "/invoices/" + fileName;

        //        return RedirectToAction("OrderSuccess");
        //    }
        //    else if (model.PaymentMethod == "PayPal")
        //    {
        //        TempData["ShippingAddress"] = finalShippingAddress;
        //        return RedirectToAction("PayWithPayPal", "Payment");
        //    }

        //    TempData["Error"] = "Invalid Payment Method!";
        //    return RedirectToAction("Checkout", "Order");
        //}




        //[HttpPost]
        //public async Task<IActionResult> PlaceOrderWithAddress(CheckoutViewModel model)
        //{
        //    Console.WriteLine($"ExistingAddress: {model.ExistingAddress}");
        //    Console.WriteLine($"ShippingAddress: {model.ShippingAddress}");

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

        //    // Use selected saved address if available
        //    string finalShippingAddress = !string.IsNullOrWhiteSpace(model.ExistingAddress)
        //        ? model.ExistingAddress
        //        : model.ShippingAddress;

        //    if (string.IsNullOrWhiteSpace(finalShippingAddress))
        //    {
        //        TempData["Error"] = "Please provide or select a shipping address.";
        //        return RedirectToAction("Checkout", "Product");
        //    }

        //    int shippingAddressId = 0;
        //    if (model.SaveAddress && !string.IsNullOrWhiteSpace(finalShippingAddress))
        //    {
        //        var savedAddress = await _userService.SaveAddress1Async(userId, finalShippingAddress);
        //        shippingAddressId = savedAddress.Id;
        //    }

        //    if (model.PaymentMethod == "COD")
        //    {
        //        var order = await _orderService.CreateOrderAsync(userId, cartItems, "COD", shippingAddressId);

        //        if (order == null)
        //        {
        //            TempData["Error"] = "Failed to place order due to out of stock";
        //            return RedirectToAction("Checkout", "Product");
        //        }

        //        // Clear Cart after order placement
        //        await _cartService.ClearCartAsync(userId);

        //        // Generate the invoice PDF for the order
        //        var invoicePdf = await _orderService.GenerateOrderInvoiceAsync(order.Id);

        //        // Save the PDF file somewhere on server or memory
        //        // (e.g. save it in a temporary file or memory stream)
        //        // var filePath = Path.Combine(Path.GetTempPath(), $"Order_{order.Id}_Invoice.pdf");
        //       var filePath= File(invoicePdf, "application/pdf", $"Order_{order.Id}_Invoice.pdf");

        //       // System.IO.File.WriteAllBytes(filePath, invoicePdf);


        //        //        var invoicePdf = await _orderService.GenerateOrderInvoiceAsync(order.Id);
        //        //        return File(invoicePdf, "application/pdf", $"Order_{order.Id}_Invoice.pdf");

        //        // Return the generated PDF to the client
        //        TempData["Success"] = "Your order has been placed successfully with Cash on Delivery!";
        //        return View("OrderSuccess", new { invoiceFilePath = filePath });
        //    }
        //    else if (model.PaymentMethod == "PayPal")
        //    {
        //        TempData["ShippingAddress"] = finalShippingAddress;
        //        return RedirectToAction("PayWithPayPal", "Payment");
        //    }

        //    TempData["Error"] = "Invalid Payment Method!";
        //    return RedirectToAction("Checkout", "Order");
        //}





        //important
        //[HttpPost]
        //public async Task<IActionResult> PlaceOrderWithAddress(CheckoutViewModel model)
        //{

        //    Console.WriteLine($"ExistingAddress: {model.ExistingAddress}");
        //    Console.WriteLine($"ShippingAddress: {model.ShippingAddress}");

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

        //    // ✅ Use selected saved address if available
        //    string finalShippingAddress = !string.IsNullOrWhiteSpace(model.ExistingAddress)
        //        ? model.ExistingAddress
        //        : model.ShippingAddress;

        //    if (string.IsNullOrWhiteSpace(finalShippingAddress))
        //    {
        //        TempData["Error"] = "Please provide or select a shipping address.";
        //        return RedirectToAction("Checkout", "Product");
        //    }

        //    int shippingAddressId = 0;
        //    if (model.SaveAddress && !string.IsNullOrWhiteSpace(finalShippingAddress))
        //    {
        //        var savedAddress = await _userService.SaveAddress1Async(userId, finalShippingAddress);
        //        shippingAddressId = savedAddress.Id;
        //    }

        //    if (model.PaymentMethod == "COD")
        //    {
        //        var order = await _orderService.CreateOrderAsync(userId, cartItems, "COD", shippingAddressId);

        //        if (order == null)
        //        {
        //            TempData["Error"] = "Failed to place order! due to out of stock";
        //            return RedirectToAction("Checkout", "Product");
        //        }

        //        await _cartService.ClearCartAsync(userId);
        //        var invoicePdf = await _orderService.GenerateOrderInvoiceAsync(order.Id);
        //        return File(invoicePdf, "application/pdf", $"Order_{order.Id}_Invoice.pdf");
        //    }
        //    else if (model.PaymentMethod == "PayPal")
        //    {
        //        TempData["ShippingAddress"] = finalShippingAddress;
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
        //    int shippingAddressId = 0;
        //    if (SaveAddress && !string.IsNullOrWhiteSpace(ShippingAddress))
        //    {
        //        var savedAddress = await _userService.SaveAddress1Async(userId, ShippingAddress);
        //        shippingAddressId = savedAddress.Id; // Address save hone ke baad uska Id assign karo
        //    }

        //    if (PaymentMethod == "COD")
        //    {
        //        // ✅ Place order with Cash on Delivery
        //        var order = await _orderService.CreateOrderAsync(userId, cartItems, "COD", shippingAddressId);

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
