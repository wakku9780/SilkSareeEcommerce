using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _context;

        public CouponRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasUserUsedCouponAsync(string userId, int couponId)
        {
            return await _context.UserCoupons
                .AnyAsync(uc => uc.UserId == userId && uc.CouponId == couponId);
        }

        public async Task SaveUserCouponAsync(string userId, int couponId)
        {
            var userCoupon = new UserCoupon
            {
                UserId = userId,
                CouponId = couponId,
                UsedAt = DateTime.UtcNow
            };
            _context.UserCoupons.Add(userCoupon);
            await _context.SaveChangesAsync();
        }


        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            return await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<List<Coupon>> GetAllAsync()
        {
            return await _context.Coupons.ToListAsync();
        }

        public async Task AddAsync(Coupon coupon)
        {
            await _context.Coupons.AddAsync(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsValidAsync(string code)
        {
            var coupon = await GetByCodeAsync(code);
            return coupon != null && coupon.IsActive && coupon.ExpiryDate > DateTime.Now;
        }
    }

}
