namespace SilkSareeEcommerce.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public string PaymentMethod { get; set; }  // COD or PayPal
        public decimal TotalAmount { get; set; }

        // ✅ Naya Address Field
        public string ShippingAddress { get; set; }

        // ✅ Checkbox ke liye
        public bool SaveAddress { get; set; }

        // ✅ Existing Address pre-fill ke liye (optional)
        public string ExistingAddress { get; set; }

        // ✅ For Dropdown
        public List<string> SavedAddresses { get; set; }
    }

}
