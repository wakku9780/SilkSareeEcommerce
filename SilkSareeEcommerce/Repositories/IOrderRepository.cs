using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);  // Add a new order
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);  // Get orders by user
        Task<Order> GetByIdAsync(int id);  // Get order by id

        Task<List<Order>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<Order> CreateOrderAsync(Order order);

        Task<Order> GetOrderWithDetailsAsync(int orderId);

        Task<Order?> GetOrderByUserAndProductAsync(string userId, int productId);

        Task<bool> HasUserPurchasedProductAsync(string userId, int productId);




    }
}
