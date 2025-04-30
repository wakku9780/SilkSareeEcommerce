namespace SilkSareeEcommerce.Models
{
    public class SavedAddress
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // User ko relate karne ke liye
        public string Address { get; set; }
        public bool IsDefault { get; set; }
    }
}
