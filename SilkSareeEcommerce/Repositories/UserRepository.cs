using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SilkSareeEcommerce.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<ApplicationUser> AddAsync(ApplicationUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<ApplicationUser> UpdateAsync(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<string> GetAddressAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.Address;
        }

        public async Task SaveAddressAsync(string userId, string address)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Address = address;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetSavedAddressesByUserIdAsync(string userId)
        {
            return await _context.SavedAddresses
                .Where(a => a.UserId == userId)
                .Select(a => a.Address)
                .ToListAsync();
        }


        // Add any other necessary methods.
    }
}
