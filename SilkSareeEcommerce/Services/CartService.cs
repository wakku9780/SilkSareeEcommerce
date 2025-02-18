using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Repositories;

namespace SilkSareeEcommerce.Services
{
    public class CartService
    {

        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        //public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId)
        //{
        //    return await _cartRepository.GetCartItemsAsync(userId);
        //}

        public async Task<IEnumerable<CartItem>> GetCartByUserIdAsync(string userId)
        {
            return await _cartRepository.GetCartItemsAsync(userId);
        }



        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            await _cartRepository.AddToCartAsync(userId, productId, quantity);
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            await _cartRepository.RemoveFromCartAsync(userId, productId);
        }

        public async Task ClearCartAsync(string userId)
        {
            await _cartRepository.ClearCartAsync(userId);
        }
    }
}
