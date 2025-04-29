using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders.Include(o => o.User).ToListAsync();
        }


        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = newStatus;
            _context.Orders.Update(order);  // ✅ Ensure entity is tracked for update
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Order?> CreateOrderAsync(Order order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products
                        .Where(p => p.Id == item.ProductId)
                        .FirstOrDefaultAsync();

                    if (product == null)
                    {
                        await transaction.RollbackAsync();
                        return null; // Product not found
                    }

                    if (product.Quantity < item.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return null; // Stock insufficient
                    }

                    product.Quantity -= item.Quantity;
                    _context.Products.Update(product);
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        //public async Task<Order> CreateOrderAsync(Order order)
        //{
        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();
        //    return order;
        //}

    }

}
