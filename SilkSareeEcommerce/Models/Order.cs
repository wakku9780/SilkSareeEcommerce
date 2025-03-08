namespace SilkSareeEcommerce.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }   // Foreign key for ApplicationUser
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; }
        public string Status { get; set; } = "Pending";  // Pending, Completed, Cancelled

        // Navigation Properties
        public ApplicationUser User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
