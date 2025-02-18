using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);  // Add a new order
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);  // Get orders by user
        Task<Order> GetByIdAsync(int id);  // Get order by id
    }
}
