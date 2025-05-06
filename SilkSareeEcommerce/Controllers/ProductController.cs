using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace SilkSareeEcommerce.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly CartService _cartService; // ✅ CartService add kiya

        private readonly PayPalService _payPalService;
        private readonly OrderService _orderService;
        private readonly UserService _userService;

        public ProductController(OrderService orderService, PayPalService payPalService,ProductService productService, CategoryService categoryService, CloudinaryService cloudinaryService, CartService cartService, UserService userService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _cloudinaryService = cloudinaryService;
            _cartService = cartService; // ✅ Injecting CartService
            _payPalService = payPalService;
            _orderService = orderService;
            _userService = userService;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var products = await _productService.GetAllProductsAsync();
        //    return View(products);
        //}


        public async Task<IActionResult> Index(string? name, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var products = await _productService.SearchProductsAsync(name, categoryId, minPrice, maxPrice);
            ViewBag.Categories = await _productService.GetAllCategoriesAsync();  // for dropdown
            return View(products);
        }



        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
            return View("crea");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("ImageUrl", "Please upload an image.");
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
                return View(product);
            }
            product.Category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);

            if (product.Category == null)
            {
                ModelState.AddModelError("CategoryId", "Invalid Category Selected");
            }

            using (var stream = imageFile.OpenReadStream())
            {
                product.ImageUrl = await _cloudinaryService.UploadImageAsync(stream, imageFile.FileName);
            }
            ModelState.Clear();
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
                return View(product);
            }

            await _productService.AddProductAsync(product);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            product.Category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);
            if (product.Category == null)
            {
                ModelState.AddModelError("CategoryId", "Invalid Category Selected");
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
                return View(product);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                using (var stream = imageFile.OpenReadStream())
                {
                    product.ImageUrl = await _cloudinaryService.UploadImageAsync(stream, imageFile.FileName);
                }
            }
            else
            {
                product.ImageUrl = existingProduct.ImageUrl;
            }

            await _productService.UpdateProductAsync(id, product);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [HttpGet("Product/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // 🛒 ✅ ADD TO CART METHOD
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _cartService.AddToCartAsync(userId, product.Id, quantity);
            return RedirectToAction(nameof(ViewCart));
        }

        // 🛒 ✅ VIEW CART METHOD
        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItems = await _cartService.GetCartItemsAsync(userId);
            return View(cartItems);
        }

        // 🛒 ✅ REMOVE FROM CART METHOD
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _cartService.RemoveFromCartAsync(userId, cartItemId);
            return RedirectToAction(nameof(ViewCart));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartQuantity(int productId, string action)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ✅ Current logged-in user ka ID le lo

            var result = await _cartService.UpdateCartQuantityAsync(userId, productId, action);

            if (result.newQuantity == -1)
            {
                return Json(new { success = false, message = "Item not found!" });
            }

            return Json(new
            {
                success = true,
                newQuantity = result.newQuantity,
                newTotal = result.newTotal.ToString("F2"),  // ✅ Price format fix
                cartTotal = result.cartTotal.ToString("F2")
            });
        }


        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var cartItems = (await _cartService.GetCartItemsAsync(userId)).ToList();

            var totalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity);
            var savedAddresses = await _userService.GetSavedAddressesAsync(userId);

            var checkoutViewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = totalAmount,
                SavedAddresses = savedAddresses.Select((address, index) => new SavedAddressDto
                {
                    Id = index + 1, // agar real DB ID nahi hai
                    Address = address
                }).ToList()
            };





            return View(checkoutViewModel);
        }








        //[HttpGet]
        //public async Task<IActionResult> Checkout()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userId)) return Unauthorized();

        //    var cartItems = (await _cartService.GetCartItemsAsync(userId)).ToList();
        //    var savedAddress = await _userService.GetAddressAsync(userId);

        //    var checkoutViewModel = new CheckoutViewModel
        //    {
        //        CartItems = cartItems,
        //        TotalAmount = cartItems.Sum(i => i.Quantity * i.Product.Price),
        //        PaymentMethod = "COD",
        //        ShippingAddress = savedAddress
        //    };

        //    return View(checkoutViewModel);
        //}

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


        [HttpGet("BuyNow/{id}")]
        public async Task<IActionResult> BuyNow(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(); // Agar product nahi mila
            }

            // BuyNowViewModel create karo aur product ko set karo
            var model = new BuyNowViewModel
            {
                Product = product
            };

            return View("BuyNow", model);  // Model ko pass karo
        }



        //[HttpGet("BuyNow/{id}")]
        //public async Task<IActionResult> BuyNow(int id)
        //{
        //    var product = await _productService.GetProductByIdAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound(); // 404 Error agar product nahi mila
        //    }

        //    return View("BuyNow", product);
        //   // return RedirectToAction(nameof(ViewCart));
        //}

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string PaymentMethod)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItems = await _cartService.GetCartItemsAsync(userId);
            if (cartItems == null || !cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction(nameof(ViewCart));
            }

            decimal totalAmount = cartItems.Sum(item => item.Quantity * item.Product.Price);

            if (PaymentMethod == "PayPal")
            {
                string returnUrl = Url.Action("PaymentSuccess", "Payment", null, Request.Scheme);
                string cancelUrl = Url.Action("PaymentCancel", "Payment", null, Request.Scheme);

                var payment = await _payPalService.CreatePaymentAsync(0,totalAmount, "USD", returnUrl, cancelUrl);
                var approvalUrl = payment.links.FirstOrDefault(l => l.rel == "approval_url")?.href;

                if (!string.IsNullOrEmpty(approvalUrl))
                    return Redirect(approvalUrl);
            }
            else if (PaymentMethod == "COD")
            {
                await _orderService.ConfirmOrderAsync(userId, cartItems);
                return RedirectToAction("OrderSuccess");
            }

            return RedirectToAction("Checkout");
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }









    }
}



//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc;
//using SilkSareeEcommerce.Models;
//using SilkSareeEcommerce.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;

//namespace SilkSareeEcommerce.Controllers
//{
//    [Authorize]

//    public class ProductController : Controller
//    {
//        private readonly ProductService _productService;
//        private readonly CategoryService _categoryService;
//        private readonly CloudinaryService _cloudinaryService;

//        public ProductController(ProductService productService, CategoryService categoryService, CloudinaryService cloudinaryService)
//        {
//            _productService = productService;
//            _categoryService = categoryService;
//            _cloudinaryService = cloudinaryService;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var products = await _productService.GetAllProductsAsync();
//            return View(products);
//        }

//        public async Task<IActionResult> Create()
//        {
//            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
//            return View();
//        }


//        [Authorize(Roles = "Admin")]
//        [HttpPost]
//        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
//        {
//            if (imageFile == null || imageFile.Length == 0)
//            {
//                ModelState.AddModelError("ImageUrl", "Please upload an image.");
//                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
//                return View(product);
//            }
//            product.Category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);

//            if (product.Category == null)
//            {
//                ModelState.AddModelError("CategoryId", "Invalid Category Selected");
//            }

//            using (var stream = imageFile.OpenReadStream())
//            {
//                product.ImageUrl = await _cloudinaryService.UploadImageAsync(stream, imageFile.FileName);
//            }
//            ModelState.Clear();
//            if (!ModelState.IsValid)
//            {
//                foreach (var error in ModelState)
//                {
//                    foreach (var err in error.Value.Errors)
//                    {
//                        Console.WriteLine($"❌ Field: {error.Key}, Error: {err.ErrorMessage}");
//                    }
//                }

//                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
//                return View(product);
//            }

//            await _productService.AddProductAsync(product);
//            return RedirectToAction(nameof(Index));
//        }


//        // ✅ EDIT PRODUCT - GET METHOD
//        [HttpGet]
//        public async Task<IActionResult> Edit(int id)
//        {
//            var product = await _productService.GetProductByIdAsync(id);
//            if (product == null)
//            {
//                return NotFound();
//            }

//            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
//            return View(product);
//        }

//        // ✅ EDIT PRODUCT - POST METHOD
//        [HttpPost]
//        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
//        {
//            if (id != product.Id)
//            {
//                return BadRequest("Product ID mismatch.");
//            }

//            var existingProduct = await _productService.GetProductByIdAsync(id);
//            if (existingProduct == null)
//            {
//                return NotFound();
//            }

//            // 🛑 Category check
//            product.Category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);
//            if (product.Category == null)
//            {
//                ModelState.AddModelError("CategoryId", "Invalid Category Selected");
//                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
//                return View(product);
//            }

//            // ✅ Image Update (Only if new image is provided)
//            if (imageFile != null && imageFile.Length > 0)
//            {
//                using (var stream = imageFile.OpenReadStream())
//                {
//                    product.ImageUrl = await _cloudinaryService.UploadImageAsync(stream, imageFile.FileName);
//                }
//            }
//            else
//            {
//                product.ImageUrl = existingProduct.ImageUrl; // 🛑 Keep old image if no new image uploaded
//            }

//            await _productService.UpdateProductAsync(id, product);
//            return RedirectToAction(nameof(Index));
//        }

//        // ✅ DELETE PRODUCT - GET METHOD (For Testing via Browser)
//        [HttpGet]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var product = await _productService.GetProductByIdAsync(id);
//            if (product == null)
//            {
//                return NotFound();
//            }

//            return View(product); // Show a confirmation page
//        }

//        // ✅ DELETE PRODUCT - POST METHOD (Actual Deletion)
//        [HttpPost]
//        public async Task<IActionResult> ConfirmDelete(int id)
//        {
//            var product = await _productService.GetProductByIdAsync(id);
//            if (product == null)
//            {
//                return NotFound();
//            }

//            await _productService.DeleteProductAsync(id);
//            return RedirectToAction(nameof(Index));
//        }

//        [AllowAnonymous] // 👈 Public access dene ke liye (Agar login required na ho)
//        [HttpGet("Product/Details/{id}")]
//        public async Task<IActionResult> Details(int id)
//        {
//            var product = await _productService.GetProductByIdAsync(id);

//            if (product == null)
//            {
//                return NotFound(); // ❌ Product nahi mila toh 404 error return kare
//            }

//            return View(product); // ✅ Product ko Details View pe bhej raha hai
//        }








//    }
//}





//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc;
//using SilkSareeEcommerce.Models;
//using SilkSareeEcommerce.Services;

//namespace SilkSareeEcommerce.Controllers
//{
//    public class ProductController : Controller
//    {
//        private readonly ProductService _productService;
//        private readonly CategoryService _categoryService;
//        private readonly CloudinaryService _cloudinaryService;

//        public ProductController(ProductService productService, CategoryService categoryService, CloudinaryService cloudinaryService)
//        {
//            _productService = productService;
//            _categoryService = categoryService;
//            _cloudinaryService = cloudinaryService;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var products = await _productService.GetAllProductsAsync();
//            return View(products);
//        }

//        public async Task<IActionResult> Create()
//        {
//            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
//        {
//            if (imageFile == null || imageFile.Length == 0)
//            {
//                ModelState.AddModelError("ImageUrl", "Please upload an image.");
//                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
//                return View(product);
//            }
//            product.Category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);

//            if (product.Category == null)
//            {
//                ModelState.AddModelError("CategoryId", "Invalid Category Selected");
//            }

//            using (var stream = imageFile.OpenReadStream())
//            {
//                product.ImageUrl = await _cloudinaryService.UploadImageAsync(stream, imageFile.FileName);
//            }
//            ModelState.Clear();
//            if (!ModelState.IsValid)
//            {
//                foreach (var error in ModelState)
//                {
//                    foreach (var err in error.Value.Errors)
//                    {
//                        Console.WriteLine($"❌ Field: {error.Key}, Error: {err.ErrorMessage}");
//                    }
//                }

//                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
//                return View(product);
//            }

//            await _productService.AddProductAsync(product);
//            return RedirectToAction(nameof(Index));
//        }

//        // ✅ EDIT PRODUCT - GET METHOD
//        public async Task<IActionResult> Edit(int id)
//        {
//            var product = await _productService.GetProductByIdAsync(id);
//            if (product == null)
//            {
//                return NotFound();
//            }

//            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
//            return View(product);
//        }

//        // ✅ EDIT PRODUCT - POST METHOD
//        [HttpPost]
//        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
//        {
//            if (id != product.Id)
//            {
//                return BadRequest("Product ID mismatch.");
//            }

//            var existingProduct = await _productService.GetProductByIdAsync(id);
//            if (existingProduct == null)
//            {
//                return NotFound();
//            }

//            // 🛑 Category check
//            product.Category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);
//            if (product.Category == null)
//            {
//                ModelState.AddModelError("CategoryId", "Invalid Category Selected");
//                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
//                return View(product);
//            }

//            // ✅ Image Update (Only if new image is provided)
//            if (imageFile != null && imageFile.Length > 0)
//            {
//                using (var stream = imageFile.OpenReadStream())
//                {
//                    product.ImageUrl = await _cloudinaryService.UploadImageAsync(stream, imageFile.FileName);
//                }
//            }
//            else
//            {
//                product.ImageUrl = existingProduct.ImageUrl; // 🛑 Keep old image if no new image uploaded
//            }


//            //////
//            ///
//            //        [HttpPost]
//            //        public async Task<IActionResult> Edit(int id, Product product)
//            //        {
//            //            if (!ModelState.IsValid)
//            //                return View(product);

//            //            await _productService.UpdateProductAsync(id, product);
//            //            return RedirectToAction(nameof(Index));
//            //        }
//            /////

//            await _productService.UpdateProductAsync(id,product);
//            return RedirectToAction(nameof(Index));
//        }

//        // ✅ DELETE PRODUCT
//        [HttpGet]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var product = await _productService.GetProductByIdAsync(id);
//            if (product == null)
//            {
//                return NotFound();
//            }

//            await _productService.DeleteProductAsync(id);
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}



//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc;
//using SilkSareeEcommerce.Models;
//using SilkSareeEcommerce.Services;
//using System.IO;


//namespace SilkSareeEcommerce.Controllers
//{

//    [Route("api/products")]
//    public class ProductController : Controller
//    {
//        private readonly ProductService _productService;
//        private readonly CategoryService _categoryService;
//        private readonly CloudinaryService _cloudinaryService;  // Cloudinary Service

//        public ProductController(ProductService productService, CategoryService categoryService, CloudinaryService cloudinaryService)
//        {
//            _productService = productService;
//            _categoryService = categoryService;
//            _cloudinaryService = cloudinaryService; // Inject Cloudinary Service
//        }

//        public async Task<IActionResult> Create()
//        {
//            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
//        {
//            if (imageFile != null && imageFile.Length > 0)
//            {
//                using (var stream = imageFile.OpenReadStream())
//                {
//                    product.ImageUrl = await _cloudinaryService.UploadImageAsync(stream, imageFile.FileName);
//                }
//            }
//            else
//            {
//                ModelState.AddModelError("ImageUrl", "Please upload an image.");
//            }

//            if (!ModelState.IsValid)
//            {
//                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
//                return View(product);
//            }

//            await _productService.AddProductAsync(product);
//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        public async Task<IActionResult> Edit(int id, [FromForm] Product product, IFormFile? imageFile)
//        {
//            var existingProduct = await _productService.GetProductByIdAsync(id);
//            if (existingProduct == null)
//                return NotFound();

//            if (imageFile != null && imageFile.Length > 0)
//            {
//                using (var stream = imageFile.OpenReadStream())
//                {
//                    product.ImageUrl = await _cloudinaryService.UploadImageAsync(stream, imageFile.FileName);
//                }
//            }
//            else
//            {
//                product.ImageUrl = existingProduct.ImageUrl; // Old image ko retain karo agar naya image nahi hai
//            }

//            await _productService.UpdateProductAsync(id, product);
//            return RedirectToAction(nameof(Index));
//        }




//    }

//}

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using SilkSareeEcommerce.Models;
//using SilkSareeEcommerce.Services;
//using System.Threading.Tasks;

//namespace SilkSareeEcommerce.Controllers
//{
//    public class ProductController : Controller
//    {
//        private readonly ProductService _productService;
//        private readonly CategoryService _categoryService; // Add this

//        public ProductController(ProductService productService, CategoryService categoryService)
//        {
//            _productService = productService;
//            _categoryService = categoryService; // Inject service
//        }


//        // Show all products in View
//        public async Task<IActionResult> Index()
//        {
//            var products = await _productService.GetAllProductsAsync();
//            return View(products);
//        }

//        // Show form to add a new product
//        //public IActionResult Create()
//        //{
//        //   // ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
//        //    return View();
//        //}


//        public async Task<IActionResult> Create()
//        {
//            var categories = await _categoryService.GetAllCategoriesAsync();

//            Console.WriteLine($"Total Categories Found: {categories.Count()}"); // Debugging

//            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
//            return View();
//        }






//        [HttpPost]
//        public async Task<IActionResult> Create(Product product)
//        {
//            Console.WriteLine($"Product Received: {product.Name}, CategoryId: {product.CategoryId}");

//            // Category fetch karo
//            product.Category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);

//            if (product.Category == null)
//            {
//                ModelState.AddModelError("CategoryId", "Invalid Category Selected");
//            }

//            // ❌ Yeh ModelState errors clear karega agar sirf Category binding ki wajah se issue aa raha ho
//            ModelState.Clear();

//            if (!ModelState.IsValid)
//            {
//                Console.WriteLine("❌ ModelState is invalid. Errors:");
//                foreach (var error in ModelState)
//                {
//                    foreach (var subError in error.Value.Errors)
//                    {
//                        Console.WriteLine($"Field: {error.Key}, Error: {subError.ErrorMessage}");
//                    }
//                }

//                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
//                return View(product);
//            }

//            await _productService.AddProductAsync(product);
//            return RedirectToAction(nameof(Index));
//        }









//        //public async Task<IActionResult> Create(Product product)
//        //{
//        //    if (!ModelState.IsValid)
//        //    {
//        //        ViewBag.Categories = new SelectList((System.Collections.IEnumerable)_categoryService.GetAllCategoriesAsync(), "Id", "Name");

//        //        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
//        //        {
//        //            Console.WriteLine("Error: " + error.ErrorMessage);
//        //        }

//        //        return View(product);
//        //    }

//        //    await _productService.AddProductAsync(product);
//        //    return RedirectToAction(nameof(Index));
//        //}





//        // Handle adding a new product
//        //[HttpPost]

//        //public async Task<IActionResult> Create(Product product)
//        //{
//        //    if (!ModelState.IsValid)
//        //    {
//        //        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
//        //        {
//        //            Console.WriteLine("Error: " + error.ErrorMessage);
//        //        }
//        //        return View(product);
//        //    }

//        //    await _productService.AddProductAsync(product);
//        //    return RedirectToAction(nameof(Index));
//        //}



//        //public async Task<IActionResult> Create(Product product)
//        //{
//        //    if (!ModelState.IsValid)
//        //        return View(product);

//        //    await _productService.AddProductAsync(product);
//        //    return RedirectToAction(nameof(Index));
//        //}

//        // Show form to edit a product
//        public async Task<IActionResult> Edit(int id)
//        {
//            var product = await _productService.GetProductByIdAsync(id);
//            if (product == null)
//                return NotFound();

//            return View(product);
//        }

//        // Handle updating a product
//        [HttpPost]
//        public async Task<IActionResult> Edit(int id, Product product)
//        {
//            if (!ModelState.IsValid)
//                return View(product);

//            await _productService.UpdateProductAsync(id, product);
//            return RedirectToAction(nameof(Index));
//        }

//        // Handle deleting a product
//        public async Task<IActionResult> Delete(int id)
//        {
//            await _productService.DeleteProductAsync(id);
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
