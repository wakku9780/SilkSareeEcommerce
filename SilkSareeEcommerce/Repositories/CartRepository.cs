using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToListAsync();
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem { UserId = userId, ProductId = productId, Quantity = quantity };
                await _context.CartItems.AddAsync(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var cartItems = _context.CartItems.Where(c => c.UserId == userId);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }


        public async Task<(int newQuantity, decimal newTotal, decimal cartTotal)> UpdateCartQuantityAsync(string userId, int productId, string action)
        {
            var cartItem = await _context.CartItems
                .Include(c => c.Product)  // ✅ Product data include karo taaki price mile
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem == null)
            {
                return (-1, 0, 0); // ✅ Item not found case handle
            }

            if (action == "increase")
            {
                cartItem.Quantity += 1;
            }
            else if (action == "decrease" && cartItem.Quantity > 1)
            {
                cartItem.Quantity -= 1;
            }

            await _context.SaveChangesAsync();

            // ✅ Calculate updated totals
            decimal newTotal = cartItem.Quantity * cartItem.Product.Price;
            decimal cartTotal = await _context.CartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity * c.Product.Price);

            return (cartItem.Quantity, newTotal, cartTotal);
        }



    }

}