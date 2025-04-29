namespace SilkSareeEcommerce.Models
{
    public class BuyNowViewModel
    {
       // public int ProductId { get; set; }   // 👈 Yeh add karo
        public Product Product { get; set; }
        public int Quantity { get; set; } = 1;
        public string PaymentMethod { get; set; } = "COD";
    }
}
