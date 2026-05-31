using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int Stock { get; set; } = 0;

        [MaxLength(100)]
        public string? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
