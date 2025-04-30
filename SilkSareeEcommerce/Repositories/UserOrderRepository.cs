using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilkSareeEcommerce.Repositories
{
    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public UserOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<Order> AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void DeleteOrder(Order order)
        {
            _context.Orders.Remove(order);
        }


      

    }
}
