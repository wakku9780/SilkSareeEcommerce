using SilkSareeEcommerce.Models;
using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Repositories;

namespace SilkSareeEcommerce.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;

        public UserService(ApplicationDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }


        public Task<string> GetAddressAsync(string userId)
        {
            return _userRepository.GetAddressAsync(userId);
        }

        public Task SaveAddressAsync(string userId, string address)
        {
            return _userRepository.SaveAddressAsync(userId, address);
        }

        public async Task<List<string>> GetSavedAddressesAsync(string userId)
        {
            return await _userRepository.GetSavedAddressesByUserIdAsync(userId);
        }


        public async Task<List<SavedAddress>> GetListSavedAddressesAsync(string userId)
        {
            return await _userRepository.GetListSavedAddressesAsync(userId);
        }

        public async Task UpdateSavedAddressesAsync(List<SavedAddress> addresses)
        {
            foreach (var address in addresses)
            {
                await _userRepository.UpdateAddressAsync(address);
            }
        }

        public async Task DeleteAddressAsync(int addressId)
        {
            await _userRepository.DeleteAddressAsync(addressId);
        }

        public async Task UpdateAddressAsync(SavedAddress address)
        {
            await _userRepository.UpdateAddressAsync(address);
        }


        public async Task UpdateUserAsync(ApplicationUser user, string userId)
        {
            await _userRepository.UpdateUserAsync(user, userId);
        }


        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }



    }
}
