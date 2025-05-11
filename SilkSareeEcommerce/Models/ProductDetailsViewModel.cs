namespace SilkSareeEcommerce.Models
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public decimal AverageRating { get; set; }
        public IEnumerable<ProductReview> Reviews { get; set; }
    }
}
