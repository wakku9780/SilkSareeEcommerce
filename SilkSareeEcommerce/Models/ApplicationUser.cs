using Microsoft.AspNetCore.Identity;

namespace SilkSareeEcommerce.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public string Address { get; set; } = string.Empty;

        // Navigation Property
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
