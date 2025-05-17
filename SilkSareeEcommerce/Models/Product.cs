using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SilkSareeEcommerce.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(1, 100000, ErrorMessage = "Price must be between 1 and 100000")]
        public decimal Price { get; set; }

        // ❌ Remove [Required]
        public string? ImageUrl { get; set; } = string.Empty;


        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        // Navigation Property
        [BindNever]  // Yeh property form se bind nahi hogi
        public Category Category { get; set; }


        [Required(ErrorMessage = "Stock is required")]
        [Range(0, 10000, ErrorMessage = "Stock must be between 0 and 10000")]
        public int Quantity { get; set; }



        [Timestamp]
        public byte[] RowVersion { get; set; } // ⚠️ This is important

        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();

        // Add average rating to Product
        public decimal AverageRating { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();


    }
}
