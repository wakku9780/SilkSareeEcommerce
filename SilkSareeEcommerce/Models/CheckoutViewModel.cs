namespace SilkSareeEcommerce.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public string PaymentMethod { get; set; }  // COD or PayPal
        public decimal TotalAmount { get; set; }

        // ✅ Discount amount and coupon code for applying discounts
        public decimal DiscountAmount { get; set; }
        public string CouponCode { get; set; }

        public string ShippingAddress { get; set; }
        public bool SaveAddress { get; set; }
        public string ExistingAddress { get; set; }
        public List<SavedAddressDto> SavedAddresses { get; set; }
        public string SelectedSavedAddress { get; set; }
    }
}
