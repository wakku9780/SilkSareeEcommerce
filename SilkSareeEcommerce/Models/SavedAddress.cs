using System.ComponentModel.DataAnnotations;

namespace SilkSareeEcommerce.Models
{
  public class SavedAddress
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Address { get; set; }

        public bool IsDefault { get; set; }
    }
}
