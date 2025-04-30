using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId)
        {
            return await _cartRepository.GetCartItemsAsync(userId);
        }

        public async Task<IEnumerable<CartItem>> GetCartByUserIdAsync(string userId)
        {
            return await _cartRepository.GetCartItemsAsync(userId);
        }



        public async Task<bool> AddToCartAsync(string userId, int productId)
        {
            // Check if item already exists in cart
            var existingCartItem = await _cartRepository.GetCartItemByUserIdAndProductIdAsync(userId, productId);
            if (existingCartItem != null)
            {
                return false; // Item already in cart
            }

            // Create new cart item
            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = 1, // Default quantity
            };

            // Add to cart repository
            await _cartRepository.AddToCartAsync(userId,productId,cartItem.Quantity);

            return true;
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
        public async Task<(int newQuantity, decimal newTotal, decimal cartTotal)> UpdateCartQuantityAsync(string userId, int productId, string action)
        {
            return await _cartRepository.UpdateCartQuantityAsync(userId, productId, action);
        }
    }
}


//using Microsoft.EntityFrameworkCore;
//using SilkSareeEcommerce.Models;
//using SilkSareeEcommerce.Repositories;

//namespace SilkSareeEcommerce.Services
//{
//    public class CartService
//    {

//        private readonly ICartRepository _cartRepository;

//        public CartService(ICartRepository cartRepository)
//        {
//            _cartRepository = cartRepository;
//        }

//        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId)
//        {
//            return await _cartRepository.GetCartItemsAsync(userId);
//        }

//        public async Task<IEnumerable<CartItem>> GetCartByUserIdAsync(string userId)
//        {
//            return await _cartRepository.GetCartItemsAsync(userId);
//        }



//        public async Task AddToCartAsync(string userId, int productId, int quantity)
//        {
//            await _cartRepository.AddToCartAsync(userId, productId, quantity);
//        }

//        public async Task RemoveFromCartAsync(string userId, int productId)
//        {
//            await _cartRepository.RemoveFromCartAsync(userId, productId);
//        }

//        public async Task ClearCartAsync(string userId)
//        {
//            await _cartRepository.ClearCartAsync(userId);
//        }
//        public async Task<(int newQuantity, decimal newTotal, decimal cartTotal)> UpdateCartQuantityAsync(string userId, int productId, string action)
//        {
//            return await _cartRepository.UpdateCartQuantityAsync(userId, productId, action);
//        }


//        //public async Task<int> UpdateCartQuantityAsync(string userId, int productId, string action)
//        //{
//        //   return await  _cartRepository.UpdateCartQuantityAsync(userId, productId, action);

//        //    //var cartItem = await _context.CartItems
//        //    //    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

//        //    //if (cartItem == null)
//        //    //{
//        //    //    return -1; // Not Found
//        //    //}

//        //    //if (action == "increase")
//        //    //{
//        //    //    cartItem.Quantity += 1;
//        //    //}
//        //    //else if (action == "decrease" && cartItem.Quantity > 1)
//        //    //{
//        //    //    cartItem.Quantity -= 1;
//        //    //}

//        //    //await _context.SaveChangesAsync();
//        //    //return cartItem.Quantity;
//        //}

//    }
//}
