using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Services;

namespace SilkSareeEcommerce.Controllers
{
    public class SearchController : Controller
    {
        private readonly ProductService _productService;

        public SearchController(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string? name, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var products = await _productService.SearchProductsAsync(name, categoryId, minPrice, maxPrice);

            ViewBag.Categories = await _productService.GetAllCategoriesAsync(); // 👈 This fixes the null error

            return View(products);
        }

    }


}
