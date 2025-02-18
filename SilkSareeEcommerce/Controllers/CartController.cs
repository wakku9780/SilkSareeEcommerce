using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Services;
using SilkSareeEcommerce.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace SilkSareeEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItem item)
        {
            // Call the AddToCartAsync method without expecting a return value
            await _cartService.AddToCartAsync(item.UserId, item.ProductId, item.Quantity);
            return Ok();
        }


        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromCart(string userId, int productId)
        {
            await _cartService.RemoveFromCartAsync(userId, productId);
            return NoContent();
        }


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCartByUserId(string userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            return Ok(cart);
        }
    }
}
