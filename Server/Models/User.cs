using System.ComponentModel.DataAnnotations;

namespace TestAuth.Server.Models
{
    public class User
    {
        public enum Roles
        {
            Administrator,
            Moderator,
            User
        }

        public int Id { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public Roles Role { get; set; } = Roles.User;
        [Required]
        public byte[]? PasswordHash { get; set; }
        [Required]
        public byte[]? PasswordSalt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime TokenExpires { get; set; }
        public DateTime Joined { get; set; } = DateTime.UtcNow;
    }
}