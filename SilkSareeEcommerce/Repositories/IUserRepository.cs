using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<ApplicationUser> AddAsync(ApplicationUser user);
        Task<ApplicationUser> UpdateAsync(ApplicationUser user);

        Task<string> GetAddressAsync(string userId);
        Task SaveAddressAsync(string userId, string address);
        // Add any other necessary methods.

        Task<List<string>> GetSavedAddressesByUserIdAsync(string userId);

    }
}
