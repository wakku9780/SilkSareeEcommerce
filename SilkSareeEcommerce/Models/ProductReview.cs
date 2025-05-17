using System.ComponentModel.DataAnnotations;

namespace SilkSareeEcommerce.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;

        // 🆕 Add UserName property
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Property to link with Product
        public Product? Product { get; set; }
        public int OrderId { get; set; }  // Link to the Order table

    }
}
