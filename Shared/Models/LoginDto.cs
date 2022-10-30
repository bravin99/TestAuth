using System.ComponentModel.DataAnnotations;

namespace TestAuth.Shared.Models
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email field is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Please enter your password to continue")]
        public string? Password { get; set; }
    }
}