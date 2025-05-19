using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> AddAsync(Product product)
        {
            product.Category = await _context.Categories.FindAsync(product.CategoryId);

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        // Save changes to the database
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Delete a product
        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
        }


        public async Task<List<Product>> SearchProductsAsync(string? name, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await query.ToListAsync();
        }


        public async Task UpdateAverageRating(int productId, double averageRating)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.AverageRating = (decimal)averageRating;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
        }



        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }


    }
}
