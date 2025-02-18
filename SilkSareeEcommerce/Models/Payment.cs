using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SilkSareeEcommerce.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }  // Primary Key

        [Required]
        public int OrderId { get; set; }  // Foreign Key (Order ke saath link hoga)

        [ForeignKey("OrderId")]
        public Order Order { get; set; }  // Navigation Property

        [Required]
        public decimal Amount { get; set; }  // Paid Amount

        [Required]
        public string PaymentMethod { get; set; }  // "Razorpay", "PayPal", etc.

        public string TransactionId { get; set; }  // Payment Gateway ka Transaction ID

        [Required]
        public string Status { get; set; }  // "Pending", "Completed", "Failed"

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;  // Payment Date
    }
}
