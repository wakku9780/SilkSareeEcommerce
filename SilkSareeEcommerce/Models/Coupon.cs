namespace SilkSareeEcommerce.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; } // e.g. FESTIVE10
        public decimal DiscountPercent { get; set; } // e.g. 10.0 for 10%
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
