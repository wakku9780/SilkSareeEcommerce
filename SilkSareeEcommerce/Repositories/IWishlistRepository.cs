using SilkSareeEcommerce.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilkSareeEcommerce.Repositories
{
    public interface IWishlistRepository
    {
        Task<IEnumerable<Wishlist>> GetWishlistByUserIdAsync(string userId);
        Task AddToWishlistAsync(Wishlist wishlist);
        Task RemoveFromWishlistAsync(int wishlistId);
        Task<bool> IsProductInWishlistAsync(string userId, int productId);

        Task ClearWishlistByUserIdAsync(string userId);

        Task<Wishlist> GetByIdAsync(int wishlistId);
    }
}
