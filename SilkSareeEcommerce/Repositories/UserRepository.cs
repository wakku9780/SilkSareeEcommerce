using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SilkSareeEcommerce.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var newAddress = new SavedAddress
            {
                UserId = userId,
                Address = address,
                IsDefault = false  // or true if it's first time
            };

            _context.SavedAddresses.Add(newAddress);
            await _context.SaveChangesAsync();
        }


        //public async Task SaveAddressAsync(string userId, string address)
        //{
        //    var user = await _context.Users.FindAsync(userId);
        //    if (user != null)
        //    {
        //        user.Address = address;
        //        await _context.SaveChangesAsync();
        //    }
        //}

        public async Task<List<string>> GetSavedAddressesByUserIdAsync(string userId)
        {
            return await _context.SavedAddresses
                .Where(a => a.UserId == userId)
                .Select(a => a.Address)
                .ToListAsync();
        }


        // Add any other necessary methods.



        public async Task<List<SavedAddress>> GetListSavedAddressesAsync(string userId)
        {
            return await _context.SavedAddresses
                                 .Where(a => a.UserId == userId)
                                 .ToListAsync();
        }

        public async Task UpdateAddressAsync(SavedAddress address)
        {
            _context.SavedAddresses.Update(address);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAddressAsync(int addressId)
        {
            var address = await _context.SavedAddresses.FindAsync(addressId);
            if (address != null)
            {
                _context.SavedAddresses.Remove(address);
                await _context.SaveChangesAsync();
            }
        }

        //public async Task UpdateUserAsync(ApplicationUser user)
        //{
        //    _context.Users.Update(user);
        //    await _context.SaveChangesAsync();
        //}




        //public async Task UpdateUserAsync(ApplicationUser user)
        //{
        //    // Ensure user is logged in or has valid ID
        //    var currentUser = await _userManager.GetUserAsync(HttpContext.User));
        //    if (currentUser == null || currentUser.Id != user.Id)
        //    {
        //        throw new Exception("User not found or ID mismatch.");
        //    }

        //    // Fetch user dynamically
        //    var existingUser = await _userManager.FindByIdAsync(currentUser.Id);
        //    if (existingUser == null)
        //    {
        //        throw new Exception("User not found.");
        //    }

        //    // Update the user
        //    existingUser.FullName = user.FullName;
        //    existingUser.Address = user.Address;
        //    existingUser.Email = user.Email;
        //    existingUser.PhoneNumber = user.PhoneNumber;

        //    await _context.SaveChangesAsync();
        //}



        public async Task UpdateUserAsync(ApplicationUser user, string userId)
        {
            if (userId != user.Id)  // Check if the IDs match
            {

                throw new Exception("User not found or ID mismatch.");
            }

            var existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser == null)
            {
                throw new Exception("User not found.");
            }

            existingUser.FullName = user.FullName;
            existingUser.Address = user.Address;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;

            await _context.SaveChangesAsync();
        }






    }
}
