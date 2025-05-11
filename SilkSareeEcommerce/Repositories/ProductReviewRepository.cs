using System;
using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.ProductReviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();
        }

        public async Task<ProductReview?> AddReviewAsync(ProductReview review)
        {
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var review = await _context.ProductReviews.FindAsync(reviewId);
            if (review == null) return false;

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
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
