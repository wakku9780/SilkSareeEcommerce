using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace SilkSareeEcommerce.Repositories
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<Product>> GetPurchasedProductsByUserAsync(string userId)
        //{
        //    return (IEnumerable<Product>)await _context.Orders
        //        .Where(o => o.UserId == userId)
        //        .Include(o => o.Products)
        //        .ThenInclude(p => p.Reviews)  // 👈 Include related reviews
        //        .Select(o => o.Products)
        //        .Distinct()
        //        .ToListAsync();
        //}


        public async Task<IEnumerable<Product>> GetPurchasedProductsByUserAsync(string userId)
        {
            // Fetch orders including products



            var orders = await _context.Orders
        .Include(o => o.OrderItems)        // Include OrderItems
            .ThenInclude(oi => oi.Product)  // Include Product within OrderItems
        .Where(o => o.UserId == userId)
        .ToListAsync();

            var products = orders
                .SelectMany(o => o.OrderItems.Select(oi => oi.Product))  // Flatten the products from OrderItems
                .DistinctBy(p => p.Id)   // Ensure unique products
                .ToList();

            foreach (var product in products)
            {
                product.Reviews = await _context.ProductReviews
                    .Where(r => r.ProductId == product.Id)
                    .ToListAsync();
            }

            return products;
            //      var orders = await _context.Orders
            //.Where(o => o.UserId == userId)
            //.Select(o => new
            //{
            //    o.Id,
            //    o.UserId,
            //    Products = o.OrderItems.Select(oi => oi.Product).ToList()
            //})
            //.ToListAsync();



            //      var products = orders
            //  .SelectMany(o => o.Products)
            //  .DistinctBy(p => p.Id)
            //  .ToList();


            //      // Attach reviews to each product
            //      foreach (var product in products)
            //      {
            //          product.Reviews = await _context.ProductReviews
            //              .Where(r => r.ProductId == product.Id)
            //              .ToListAsync();
            //      }

            //      return products;
        }






        //public async Task<IEnumerable<Product>> GetPurchasedProductsByUserAsync(string userId)
        //{
        //    return await _context.Products
        //        .Include(p => p.Reviews)
        //        .Where(p => p.Orders.Any(o => o.UserId == userId)) // Check if the product was ordered by the user
        //        .ToListAsync();
        //}


        public async Task<bool> HasPurchasedProductAsync(Guid userId, int productId)
        {

            string userIdString = userId.ToString();
            return await _context.Orders
                .Include(o => o.OrderItems)
                .AnyAsync(o => o.UserId == userIdString && o.OrderItems.Any(oi => oi.ProductId == productId));
        }


        public async Task<IEnumerable<ProductReview>> GetReviewsAsync(int productId)
        {
            return await _context.ProductReviews
                                 .Where(r => r.ProductId == productId)
                                 .ToListAsync();
        }

        public async Task<bool> IsVerifiedBuyerAsync(string userId, int productId)
        {
            return await _context.Orders
                .AnyAsync(o => o.UserId == userId && o.OrderItems.Any(oi => oi.ProductId == productId));
        }

        public async Task AddReviewAsync(ProductReview review)
        {
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _context.ProductReviews.FindAsync(id);
            if (review != null)
            {
                _context.ProductReviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(ProductReview review)
        {
            throw new NotImplementedException();
        }
    }
}


//using SilkSareeEcommerce.Data;
//using SilkSareeEcommerce.Models;
//using Microsoft.EntityFrameworkCore;

//namespace SilkSareeEcommerce.Repositories
//{
//    public class ProductReviewRepository : IProductReviewRepository
//    {
//        private readonly ApplicationDbContext _context;

//        public ProductReviewRepository(ApplicationDbContext context)
//        {
//            _context = context;
//        }


//        public async Task<ProductReview> AddAsync(ProductReview review)
//        {
//            await _context.ProductReviews.AddAsync(review);
//            await _context.SaveChangesAsync();
//            return review;
//        }

//        public async Task<List<ProductReview>> GetByProductIdAsync(int productId)
//        {
//            return await _context.ProductReviews
//                .Where(r => r.ProductId == productId)
//                .ToListAsync();
//        }

//        //public async Task<ProductReview> AddAsync(ProductReview review)
//        //{
//        //    await _context.ProductReviews.AddAsync(review);
//        //    await _context.SaveChangesAsync();
//        //    return review;
//        //}

//        //public async Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId)
//        //{
//        //    return await _context.ProductReviews
//        //        .Where(r => r.ProductId == productId)
//        //        .ToListAsync();
//        //}

//        //public async Task DeleteAsync(int id)
//        //{
//        //    var review = await _context.ProductReviews.FindAsync(id);
//        //    if (review != null)
//        //    {
//        //        _context.ProductReviews.Remove(review);
//        //        await _context.SaveChangesAsync();
//        //    }
//        //}
//    }
//}
