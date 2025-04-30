using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilkSareeEcommerce.Services
{
    public class WishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<IEnumerable<Wishlist>> GetWishlistByUserIdAsync(string userId)
        {
            return await _wishlistRepository.GetWishlistByUserIdAsync(userId);
        }

        public async Task<bool> AddToWishlistAsync(string userId, int productId)
        {
            bool alreadyExists = await _wishlistRepository.IsProductInWishlistAsync(userId, productId);
            if (alreadyExists)
                return false;  // Product already in wishlist

            var wishlistItem = new Wishlist
            {
                UserId = userId,
                ProductId = productId
            };

            await _wishlistRepository.AddToWishlistAsync(wishlistItem);
            return true;
        }

        public async Task RemoveFromWishlistAsync(int wishlistId)
        {
            await _wishlistRepository.RemoveFromWishlistAsync(wishlistId);
        }

        public async Task ClearWishlistAsync(string userId)
        {
            await _wishlistRepository.ClearWishlistByUserIdAsync(userId);
        }

        // Yeh method wishlist mein item get karne ke liye
        public async Task<Wishlist> GetWishlistItemByIdAsync(int wishlistId)
        {
            return await _wishlistRepository.GetByIdAsync(wishlistId);
        }


         

    }
}
