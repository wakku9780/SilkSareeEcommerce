using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Services;

namespace SilkSareeEcommerce.Controllers
{
    public class ProductReviewController : Controller
    {
        private readonly ProductReviewService _reviewService;

        public ProductReviewController(ProductReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // ✅ Get All Purchased Products for Review
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Get logged-in user ID
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var purchasedProducts = await _reviewService.GetPurchasedProductsByUserAsync(userId);

            if (purchasedProducts == null || !purchasedProducts.Any())
            {
                ViewBag.Message = "You have not purchased any products yet.";
                return View(new List<Product>());  // Return empty list
            }

            // Debugging product details
            foreach (var product in purchasedProducts)
            {
                Console.WriteLine($"Product ID: {product.Id}, Name: {product.Name}");
            }

            return View("Index", purchasedProducts);
        }



        [HttpGet]
        public IActionResult Add(int productId)
        {
            var model = new ProductReview
            {
                ProductId = productId
            };
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Add(ProductReview model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.Identity.Name;

                var review = new ProductReview
                {
                    Rating = model.Rating,
                    Comment = model.Comment,
                    ProductId = model.ProductId,
                    UserId = userId,
                    UserName = userName,
                    CreatedAt = DateTime.UtcNow
                };

                await _reviewService.AddReviewAsync(review);
                return RedirectToAction("Index", new { productId = model.ProductId });
            }

            return View(model);
        }

    }
}





//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SilkSareeEcommerce.Data;
//using SilkSareeEcommerce.Models;
//using SilkSareeEcommerce.Services;
//using System.Security.Claims;

//namespace SilkSareeEcommerce.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProductReviewController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserService _userService;

//        public ProductReviewController(ApplicationDbContext context, UserService userService)
//        {
//            _context = context;
//            _userService = userService;
//        }

//        // Add Review
//        [HttpPost("AddReview")]
//        public async Task<IActionResult> AddReview(ProductReview review)
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            if (string.IsNullOrEmpty(userId)) return Unauthorized();

//            // Ensure user is a verified buyer
//            var isVerifiedBuyer = await _userService.IsVerifiedBuyerAsync(userId, review.ProductId);
//            if (!isVerifiedBuyer)
//            {
//                return BadRequest("Only verified buyers can leave a review.");
//            }

//            review.UserId = userId;
//            review.CreatedDate = DateTime.Now;

//            _context.ProductReviews.Add(review);
//            await _context.SaveChangesAsync();

//            // Update average rating for the product
//            var product = await _context.Products.FindAsync(review.ProductId);
//            product.AverageRating = (decimal)_context.ProductReviews
//                .Where(r => r.ProductId == review.ProductId)
//                .Average(r => r.Rating);

//            _context.Products.Update(product);
//            await _context.SaveChangesAsync();

//            return Ok(review);
//        }

//        // Get Reviews for a Product
//        [HttpGet("GetReviews/{productId}")]
//        public async Task<IActionResult> GetReviews(int productId)
//        {
//            var reviews = await _context.ProductReviews
//                .Where(r => r.ProductId == productId)
//                .ToListAsync();

//            return Ok(reviews);
//        }

//        // Edit Review
//        [HttpPut("EditReview/{id}")]
//        public async Task<IActionResult> EditReview(int id, ProductReview updatedReview)
//        {
//            var review = await _context.ProductReviews.FindAsync(id);
//            if (review == null) return NotFound();

//            // Ensure that the user is the one who created the review
//            if (review.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
//            {
//                return Unauthorized();
//            }

//            review.Rating = updatedReview.Rating;
//            review.ReviewText = updatedReview.ReviewText;

//            _context.ProductReviews.Update(review);
//            await _context.SaveChangesAsync();

//            // Recalculate the average rating for the product
//            var product = await _context.Products.FindAsync(review.ProductId);
//            product.AverageRating = (decimal)_context.ProductReviews
//                .Where(r => r.ProductId == review.ProductId)
//                .Average(r => r.Rating);

//            _context.Products.Update(product);
//            await _context.SaveChangesAsync();

//            return Ok(review);
//        }

//        // Delete Review
//        [HttpDelete("DeleteReview/{id}")]
//        public async Task<IActionResult> DeleteReview(int id)
//        {
//            var review = await _context.ProductReviews.FindAsync(id);
//            if (review == null) return NotFound();

//            // Ensure that the user is the one who created the review
//            if (review.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
//            {
//                return Unauthorized();
//            }

//            _context.ProductReviews.Remove(review);
//            await _context.SaveChangesAsync();

//            // Recalculate the average rating for the product
//            var product = await _context.Products.FindAsync(review.ProductId);
//            product.AverageRating = (decimal)_context.ProductReviews
//                .Where(r => r.ProductId == review.ProductId)
//                .Average(r => r.Rating);

//            _context.Products.Update(product);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}
