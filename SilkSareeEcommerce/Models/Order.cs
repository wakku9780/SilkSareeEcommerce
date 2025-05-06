using System.ComponentModel.DataAnnotations.Schema;

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

        // Use SavedAddress as a reference
        public int ShippingAddressId { get; set; }

        [ForeignKey("ShippingAddressId")]
        public SavedAddress ShippingAddress { get; set; }

    }
}
