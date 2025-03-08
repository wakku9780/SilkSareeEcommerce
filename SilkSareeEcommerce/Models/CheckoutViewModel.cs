namespace SilkSareeEcommerce.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public string PaymentMethod { get; set; }  // COD or PayPal
        public decimal TotalAmount { get; set; }

    }
}
