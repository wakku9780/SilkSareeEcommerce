using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Services;
using System.Threading.Tasks;

namespace SilkSareeEcommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService; // Add this

        public ProductController(ProductService productService, CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService; // Inject service
        }


        // Show all products in View
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // Show form to add a new product
        //public IActionResult Create()
        //{
        //   // ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
        //    return View();
        //}


        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            Console.WriteLine($"Total Categories Found: {categories.Count()}"); // Debugging

            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
            return View();
        }






        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            Console.WriteLine($"Product Received: {product.Name}, CategoryId: {product.CategoryId}");

            // Category fetch karo
            product.Category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);

            if (product.Category == null)
            {
                ModelState.AddModelError("CategoryId", "Invalid Category Selected");
            }

            // ❌ Yeh ModelState errors clear karega agar sirf Category binding ki wajah se issue aa raha ho
            ModelState.Clear();

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState is invalid. Errors:");
                foreach (var error in ModelState)
                {
                    foreach (var subError in error.Value.Errors)
                    {
                        Console.WriteLine($"Field: {error.Key}, Error: {subError.ErrorMessage}");
                    }
                }

                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
                return View(product);
            }

            await _productService.AddProductAsync(product);
            return RedirectToAction(nameof(Index));
        }









        //public async Task<IActionResult> Create(Product product)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.Categories = new SelectList((System.Collections.IEnumerable)_categoryService.GetAllCategoriesAsync(), "Id", "Name");

        //        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        //        {
        //            Console.WriteLine("Error: " + error.ErrorMessage);
        //        }

        //        return View(product);
        //    }

        //    await _productService.AddProductAsync(product);
        //    return RedirectToAction(nameof(Index));
        //}





        // Handle adding a new product
        //[HttpPost]

        //public async Task<IActionResult> Create(Product product)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        //        {
        //            Console.WriteLine("Error: " + error.ErrorMessage);
        //        }
        //        return View(product);
        //    }

        //    await _productService.AddProductAsync(product);
        //    return RedirectToAction(nameof(Index));
        //}



        //public async Task<IActionResult> Create(Product product)
        //{
        //    if (!ModelState.IsValid)
        //        return View(product);

        //    await _productService.AddProductAsync(product);
        //    return RedirectToAction(nameof(Index));
        //}

        // Show form to edit a product
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // Handle updating a product
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            await _productService.UpdateProductAsync(id, product);
            return RedirectToAction(nameof(Index));
        }

        // Handle deleting a product
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
