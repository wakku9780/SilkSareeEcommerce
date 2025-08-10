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
                // ✅ Stock check and reduce
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

                Console.WriteLine("ShippingAddressId: " + order.ShippingAddressId);

                // ✅ Shipping Address check
                if (order.ShippingAddressId > 0)
                {
                    var existingAddress = await _context.SavedAddresses
                        .FirstOrDefaultAsync(a => a.Id == order.ShippingAddressId);

                    if (existingAddress == null)
                    {
                        throw new Exception($"Invalid ShippingAddressId: {order.ShippingAddressId}. Address not found.");
                    }
                }
                else if (order.ShippingAddress != null)
                {
                    _context.SavedAddresses.Add(order.ShippingAddress);
                    await _context.SaveChangesAsync();
                    order.ShippingAddressId = order.ShippingAddress.Id;
                }
                else
                {
                    throw new Exception("No valid ShippingAddressId or ShippingAddress object provided.");
                }

                // ✅ Place order
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order ?? throw new InvalidOperationException("Failed to create order");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Order saving failed: " + ex.Message);
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


        public async Task<Order> GetOrderWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetOrderByUserAndProductAsync(string userId, int productId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.OrderItems.Any(oi => oi.ProductId == productId));
        }


        public async Task<bool> HasUserPurchasedProductAsync(string userId, int productId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .AnyAsync(o => o.UserId == userId && o.OrderItems.Any(oi => oi.ProductId == productId));
        }





    }

}
