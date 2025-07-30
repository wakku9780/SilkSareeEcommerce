using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public interface ICouponRepository
    {
        Task<Coupon?> GetByCodeAsync(string code);
        Task<List<Coupon>> GetAllAsync();
        Task AddAsync(Coupon coupon);
        Task<bool> IsValidAsync(string code);
    }
}
