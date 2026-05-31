using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty; //unique user
        [Required, MaxLength(100)]
        public string Nickname { get; set; } = string.Empty; //nickname

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User"; // "User" or "Admin"

        public List<Cart> Carts { get; set; } = new();
    }
}
