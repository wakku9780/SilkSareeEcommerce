using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId);
        Task AddToCartAsync(string userId, int productId, int quantity);
        Task RemoveFromCartAsync(string userId, int productId);
        Task ClearCartAsync(string userId);
        Task<(int newQuantity, decimal newTotal, decimal cartTotal)> UpdateCartQuantityAsync(string userId, int productId, string action);
        Task<CartItem> GetCartItemByUserIdAndProductIdAsync(string userId, int productId);

    }

}