using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public interface IProductReviewRepository
    {
        //Task<IEnumerable<ProductReview>> GetReviewsByProductIdAsync(int productId);
        //Task AddReviewAsync(ProductReview review);
        //Task<ProductReview?> GetReviewByIdAsync(int id);
        //Task UpdateReviewAsync(ProductReview review);
        //Task DeleteReviewAsync(int id);

        //Task<ProductReview> AddAsync(ProductReview review);
        //Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId);
        //Task DeleteAsync(int id);


        //Task<ProductReview> AddAsync(ProductReview review);
        //Task<List<ProductReview>> GetByProductIdAsync(int productId);

        Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId);
        Task<ProductReview?> AddReviewAsync(ProductReview review);
        Task<bool> DeleteReviewAsync(int reviewId);

    }
}
