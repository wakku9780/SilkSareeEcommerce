namespace SilkSareeEcommerce.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Add the Description property
        public string Description { get; set; } = string.Empty;  // Add this line

        // Navigation Property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
