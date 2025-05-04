using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);  // Add a new product
        Task<List<Product>> GetAllAsync();  // Get all products
        Task<Product> GetByIdAsync(int id);  // Get product by id
        Task SaveAsync();  // Save changes to the database
        void DeleteProduct(Product product);  // Delete a product

        Task<List<Category>> GetAllCategoriesAsync();


        Task<List<Product>> SearchProductsAsync(string? name, int? categoryId, decimal? minPrice, decimal? maxPrice);

    }

}
