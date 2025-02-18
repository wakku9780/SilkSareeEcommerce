using System.ComponentModel.DataAnnotations;

namespace SilkSareeEcommerce.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; }  // Token received in reset link

        [Required]
        [EmailAddress]
        public string Email { get; set; }  // User's email address

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }  // New Password

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }  // Confirm Password
    }
}
