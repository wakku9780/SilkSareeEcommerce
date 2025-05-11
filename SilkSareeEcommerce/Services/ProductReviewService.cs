using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Repositories;

namespace SilkSareeEcommerce.Services
{
    public class ProductReviewService
    {
        private readonly IProductReviewRepository _reviewRepository;

        public ProductReviewService(IProductReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<ProductReview>> GetReviewsAsync(int productId)
        {
            return await _reviewRepository.GetReviewsByProductIdAsync(productId);
        }

        public async Task<ProductReview?> AddReviewAsync(ProductReview review)
        {

            if (review.ProductId == 0)
            {
                throw new ArgumentException("Product ID cannot be zero.");
            }
            return await _reviewRepository.AddReviewAsync(review);
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            return await _reviewRepository.DeleteReviewAsync(reviewId);
        }
    }
}



//using SilkSareeEcommerce.Models;
//using SilkSareeEcommerce.Repositories;

//namespace SilkSareeEcommerce.Services
//{
//    public class ProductReviewService
//    {
//        private readonly IProductReviewRepository _reviewRepository;

//        public ProductReviewService(IProductReviewRepository reviewRepository)
//        {
//            _reviewRepository = reviewRepository;
//        }


//        public async Task<ProductReview> AddReviewAsync(ReviewDto reviewDto)
//        {
//            var review = new ProductReview
//            {
//                ProductId = reviewDto.ProductId,
//                UserName = reviewDto.UserName,
//                Rating = reviewDto.Rating,
//                ReviewText = reviewDto.ReviewText
//            };

//            return await _reviewRepository.AddAsync(review);
//        }

//        public async Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId)
//        {
//            return await _reviewRepository.GetByProductIdAsync(productId);
//        }

//        //public async Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId)
//        //{
//        //    return await _reviewRepository.GetReviewsByProductIdAsync(productId);
//        //}

//        //public async Task<ProductReview> AddReviewAsync(ProductReview review)
//        //{
//        //    return await _reviewRepository.AddAsync(review);
//        //}

//        //public async Task DeleteReviewAsync(int id)
//        //{
//        //    await _reviewRepository.DeleteAsync(id);
//        //}
//    }
//}
