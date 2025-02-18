using System.ComponentModel.DataAnnotations;

namespace SilkSareeEcommerce.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
