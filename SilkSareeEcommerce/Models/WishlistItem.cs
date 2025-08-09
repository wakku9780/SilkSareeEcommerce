using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SilkSareeEcommerce.Models
{
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }  // Foreign Key for User

        [Required]
        public int ProductId { get; set; }  // Foreign Key for Product

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
