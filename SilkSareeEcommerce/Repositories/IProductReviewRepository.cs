using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public interface IProductReviewRepository
    {
        Task<IEnumerable<ProductReview>> GetReviewsAsync(int productId);
        Task<bool> IsVerifiedBuyerAsync(string userId, int productId);
        Task AddReviewAsync(ProductReview review);
        Task DeleteReviewAsync(int id);
        Task<IEnumerable<Product>> GetPurchasedProductsByUserAsync(string userId);
        Task<bool> HasPurchasedProductAsync(Guid userId, int productId);

        // Task UpdateAverageRating(int productId, double averageRating);

        Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId);
        Task AddAsync(ProductReview review);





    }
}
