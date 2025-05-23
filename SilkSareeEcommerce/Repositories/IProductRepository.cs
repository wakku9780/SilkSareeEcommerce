﻿using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public interface IProductRepository
    {


        //Task<ProductReview> AddAsync(ProductReview review);
        //Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId);
        //Task DeleteAsync(int id);


        Task<Product> AddAsync(Product product);  // Add a new product
        Task<List<Product>> GetAllAsync();  // Get all products
        Task<Product> GetByIdAsync(int id);  // Get product by id
        Task SaveAsync();  // Save changes to the database
        void DeleteProduct(Product product);  // Delete a product

        Task<List<Category>> GetAllCategoriesAsync();


        Task<List<Product>> SearchProductsAsync(string? name, int? categoryId, decimal? minPrice, decimal? maxPrice);

        Task UpdateAverageRating(int productId, double averageRating);


    }

}
