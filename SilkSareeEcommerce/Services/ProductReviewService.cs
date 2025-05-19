using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Repositories;

namespace SilkSareeEcommerce.Services
{
    public class ProductReviewService
    {
        private readonly IProductReviewRepository _repository;
        private readonly IProductRepository _productRepository;

        public ProductReviewService(IProductReviewRepository repository, IProductRepository productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetPurchasedProductsByUserAsync(string userId)
        {
            return await _repository.GetPurchasedProductsByUserAsync(userId);
        }



        public async Task<bool> CanUserReviewAsync(Guid userId, int productId)
        {
            return await _repository.HasPurchasedProductAsync(userId, productId);
        }

        public async Task<IEnumerable<ProductReview>> GetReviewsAsync(int productId)
        {
            return await _repository.GetReviewsAsync(productId);
        }

        //public async Task<bool> AddReviewAsync(ProductReview review)
        //{
        //    var isVerified = await _repository.IsVerifiedBuyerAsync(review.UserId, review.ProductId);
        //    if (isVerified)
        //    {
        //        await _repository.AddReviewAsync(review);
        //        return true;
        //    }
        //    return false;
        //}

        public async Task DeleteReviewAsync(int id)
        {
            await _repository.DeleteReviewAsync(id);
        }

        // ✅ Get purchased products for the current user
        public async Task<IEnumerable<Product>> GetPurchasedProductsAsync(string userId)
        {
            return await _repository.GetPurchasedProductsByUserAsync(userId);
        }

        // ✅ Add a new review
        public async Task AddReviewAsync(ProductReview review)
        {
            await _repository.AddReviewAsync(review);
        }

        public async Task<decimal> GetAverageRatingAsync(int productId)
        {
            var reviews = await _repository.GetReviewsAsync(productId);

            if (reviews == null || !reviews.Any())
                return 0;

            // Calculate average rating
            return (decimal)reviews.Average(r => r.Rating);
        }

          public async Task UpdateAverageRatingAsync(int productId)
    {
        var reviews = await _repository.GetByProductIdAsync(productId);
        if (reviews != null && reviews.Any())
        {
            var averageRating = reviews.Average(r => r.Rating);
            await _productRepository.UpdateAverageRating(productId, averageRating);
        }
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
