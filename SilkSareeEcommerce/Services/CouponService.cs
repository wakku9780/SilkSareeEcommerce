using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Repositories;

namespace SilkSareeEcommerce.Services
{
    public class CouponService
    {
        private readonly ICouponRepository _couponRepo;

        public CouponService(ICouponRepository couponRepo)
        {
            _couponRepo = couponRepo;
        }

        //public async Task<Coupon?> ApplyCouponAsync(string code, decimal cartTotal, string userId)
        //{
        //    var coupon = await _couponRepo.GetByCodeAsync(code);

        //    if (coupon == null || !coupon.IsActive || coupon.ExpiryDate <= DateTime.Now)
        //        return null;

        //    // 🔴 Check if already used by this user
        //    bool alreadyUsed = await _couponRepo.HasUserUsedCouponAsync(userId, coupon.Id);
        //    if (alreadyUsed)
        //        return null;

        //    // ✅ Valid coupon
        //    return coupon;
        //}


        public async Task<Coupon?> ApplyCouponAsync(string code, decimal cartTotal)
        {
            var coupon = await _couponRepo.GetByCodeAsync(code);
            if (coupon == null || !coupon.IsActive || coupon.ExpiryDate <= DateTime.UtcNow)
                return null;

            return coupon;
        }

        public async Task<List<Coupon>> GetAllCouponsAsync()
        {
            return await _couponRepo.GetAllAsync();
        }

        public async Task CreateCouponAsync(Coupon coupon)
        {
            await _couponRepo.AddAsync(coupon);
        }
    }
}
