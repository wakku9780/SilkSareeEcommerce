using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilkSareeEcommerce.Services
{
    public class UserOrderService  
    {
        private readonly UserOrderRepository _userOrderRepository;

        public UserOrderService(UserOrderRepository userOrderRepository)
        {
            _userOrderRepository = userOrderRepository;
        }



        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _userOrderRepository.GetAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _userOrderRepository.GetByIdAsync(id);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            return await _userOrderRepository.AddAsync(order);
        }

        public async Task<Order> UpdateOrderAsync(int id, Order order)
        {
            var existingOrder = await _userOrderRepository.GetByIdAsync(id);
            if (existingOrder == null)
            {
                return null;
            }

            existingOrder.Status = order.Status;
            existingOrder.TotalAmount = order.TotalAmount;

            await _userOrderRepository.SaveAsync();
            return existingOrder;
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _userOrderRepository.GetByIdAsync(id);
            if (order != null)
            {
                _userOrderRepository.DeleteOrder(order);
                await _userOrderRepository.SaveAsync();
            }
        }
    }
}
