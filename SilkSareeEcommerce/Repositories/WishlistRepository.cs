using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilkSareeEcommerce.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Wishlist>> GetWishlistByUserIdAsync(string userId)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .ToListAsync();
        }

        public async Task AddToWishlistAsync(Wishlist wishlist)
        {
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromWishlistAsync(int wishlistId)
        {
            var wishlistItem = await _context.Wishlists.FindAsync(wishlistId);
            if (wishlistItem != null)
            {
                _context.Wishlists.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsProductInWishlistAsync(string userId, int productId)
        {
            return await _context.Wishlists.AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }
    }
}
