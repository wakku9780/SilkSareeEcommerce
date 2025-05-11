using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Services;
using System.Security.Claims;

namespace SilkSareeEcommerce.Controllers
{
    public class ProductReviewController : Controller
    {
        private readonly ProductReviewService _productReviewService;

        public ProductReviewController(ProductReviewService productReviewService)
        {
            _productReviewService = productReviewService;
        }

        //public async Task<IActionResult> Index(int productId)
        //{
        //    var reviews = await _productReviewService.GetReviewsAsync(productId);
        //    return View(reviews);
        //}

        public async Task<IActionResult> Index(int productId)
        {
            var reviews = await _productReviewService.GetReviewsAsync(productId);
            ViewBag.ProductId = productId;  // Pass the ProductId to the view
            return View(reviews);
        }

        [HttpGet]
        public IActionResult AddReview(int productId)
        {
            ViewData["ProductId"] = productId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(ProductReview review)
        {
            // Check if the ProductId is correctly received
            if (review.ProductId == 0)
            {
                ModelState.AddModelError("", "Invalid Product ID.");
                return View(review);
            }

            // Set UserId from the logged-in user
            review.UserId = User.Identity.Name ?? "Anonymous";

            if (ModelState.IsValid)
            {
                try
                {
                    await _productReviewService.AddReviewAsync(review);
                    return RedirectToAction("ProductDetails", "Product", new { id = review.ProductId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error while adding review: " + ex.Message);
                }
            }

            return View(review);
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
